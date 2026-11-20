using AutoMapper;
using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Helpers;
using GolBet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Services.Implementations;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IBetRepository _betRepository;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<Team> _teamRepository;
    private readonly AppDbContext _context;

    public MatchService(
        IMatchRepository matchRepository,
        IBetRepository betRepository,
        IMapper mapper,
        IGenericRepository<Team> teamRepository,
        AppDbContext context)
    {
        _matchRepository = matchRepository;
        _betRepository = betRepository;
        _mapper = mapper;
        _teamRepository = teamRepository;
        _context = context;
    }

    public async Task<IEnumerable<MatchDto>> GetBoardAsync(MatchStatus? status = null)
    {
        var matches = await _matchRepository.GetAllWithTeamsAsync(status);
        var dtos = _mapper.Map<List<MatchDto>>(matches);

        var matchList = matches.ToList();
        for (int i = 0; i < dtos.Count && i < matchList.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(matchList[i].HomeTeam?.PrimaryColor))
                dtos[i].HomeTeamColor = matchList[i].HomeTeam.PrimaryColor;

            if (!string.IsNullOrWhiteSpace(matchList[i].AwayTeam?.PrimaryColor))
                dtos[i].AwayTeamColor = matchList[i].AwayTeam.PrimaryColor;
        }

        return dtos;
    }

    public async Task<MatchDetailDto?> GetDetailAsync(int id)
    {
        var match = await _matchRepository.GetByIdWithDetailsAsync(id);
        return match is null ? null : _mapper.Map<MatchDetailDto>(match);
    }

    public async Task<MatchFormDto?> GetForEditAsync(int id)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        return match is null || !match.IsActive ? null : _mapper.Map<MatchFormDto>(match);
    }

    public async Task CreateAsync(MatchFormDto dto)
    {
        await ValidateBusinessRules(dto);
        var match = _mapper.Map<Match>(dto);
        match.Date = dto.Date.ToUtcFromColombia();
        match.Status = MatchStatus.Scheduled;
        match.IsActive = true;
        await _matchRepository.AddAsync(match);
    }

    public async Task UpdateAsync(MatchFormDto dto)
    {
        var match = await _matchRepository.GetByIdAsync(dto.Id);
        if (match is null || !match.IsActive)
            throw new InvalidOperationException("No se encontró el partido activo.");
        if (match.Status != MatchStatus.Scheduled)
            throw new InvalidOperationException("Solo se pueden editar partidos programados.");

        await ValidateBusinessRules(dto);
        match.HomeTeamId = dto.HomeTeamId;
        match.AwayTeamId = dto.AwayTeamId;
        match.Date = dto.Date.ToUtcFromColombia();
        match.HomeOdds = dto.HomeOdds;
        match.DrawOdds = dto.DrawOdds;
        match.AwayOdds = dto.AwayOdds;
        await _matchRepository.UpdateAsync(match);
    }

    public async Task DeactivateAsync(int id)
    {
        var match = await _matchRepository.GetByIdWithDetailsAsync(id);
        if (match is null || !match.IsActive)
            throw new InvalidOperationException("No se encontró el partido activo.");
        if (match.Bets.Count > 0)
            throw new InvalidOperationException("No se puede desactivar un partido que ya tiene apuestas.");
        await _matchRepository.DeactivateAsync(id);
    }

    public async Task StartMatchAsync(int id)
    {
        var match = await _matchRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Match {id} not found.");

        if (match.Status != MatchStatus.Scheduled)
            throw new InvalidOperationException("Solo un partido programado puede iniciarse.");

        match.Status = MatchStatus.InProgress;
        await _matchRepository.UpdateAsync(match);
    }

    public async Task<ResolutionSummaryDto> CloseMatchAsync(MatchResultFormDto dto)
    {
        var match = await _matchRepository.GetByIdAsync(dto.MatchId)
            ?? throw new KeyNotFoundException($"Match {dto.MatchId} not found.");

        if (match.Status == MatchStatus.Finished)
            throw new InvalidOperationException("Este partido ya fue cerrado y sus apuestas resueltas.");

        if (match.Status != MatchStatus.InProgress)
            throw new InvalidOperationException("El partido debe estar «En juego» para cerrarse. Inícialo primero.");

        match.HomeGoals = dto.HomeGoals;
        match.AwayGoals = dto.AwayGoals;
        match.Status = MatchStatus.Finished;

        var winningPick = GetWinningPick(dto.HomeGoals, dto.AwayGoals);
        var pendingBets = await _betRepository.GetPendingByMatchAsync(match.Id);
        var summary = new ResolutionSummaryDto { ResolvedBets = pendingBets.Count };

        foreach (var bet in pendingBets)
        {
            if (bet.Pick == winningPick)
            {
                bet.Status = BetStatus.Won;
                var payout = bet.Amount * bet.OddsAtPlacement;
                bet.User.Balance += payout;
                summary.WonBets++;
                summary.TotalPaid += payout;
            }
            else
            {
                bet.Status = BetStatus.Lost;
            }
        }

        await _matchRepository.UpdateAsync(match);
        return summary;
    }

    private static BetPick GetWinningPick(int homeGoals, int awayGoals)
        => homeGoals > awayGoals ? BetPick.Home
         : homeGoals < awayGoals ? BetPick.Away
         : BetPick.Draw;

    private async Task ValidateBusinessRules(MatchFormDto dto)
    {
        if (dto.HomeTeamId == dto.AwayTeamId)
            throw new InvalidOperationException("El equipo local y el visitante deben ser diferentes.");
        if (dto.Date.ToUtcFromColombia() <= DateTime.UtcNow)
            throw new InvalidOperationException("La fecha del partido debe estar en el futuro.");
        if (dto.HomeOdds <= 1m || dto.DrawOdds <= 1m || dto.AwayOdds <= 1m)
            throw new InvalidOperationException("Todas las cuotas deben ser mayores que 1.00.");

        var home = await _teamRepository.GetByIdAsync(dto.HomeTeamId);
        var away = await _teamRepository.GetByIdAsync(dto.AwayTeamId);
        if (home is null || away is null || !home.IsActive || !away.IsActive)
            throw new InvalidOperationException("Selecciona dos equipos activos.");
    }

    public async Task CloseMatchAndResolveBetsAsync(int matchId, int homeGoals, int awayGoals)
    {
        var match = await _context.Matches
            .Include(m => m.Bets)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null || match.Status == MatchStatus.Finished) return;

        match.HomeGoals = homeGoals;
        match.AwayGoals = awayGoals;
        match.Status = MatchStatus.Finished;

        var winningPick = homeGoals > awayGoals
            ? BetPick.Home
            : (homeGoals < awayGoals ? BetPick.Away : BetPick.Draw);

        foreach (var bet in match.Bets.Where(b => b.Status == BetStatus.Pending))
        {
            if (bet.Pick == winningPick)
            {
                bet.Status = BetStatus.Won;
                bet.User.Balance += bet.Amount * bet.OddsAtPlacement;
            }
            else
            {
                bet.Status = BetStatus.Lost;
            }
        }

        await _context.SaveChangesAsync();
    }
}