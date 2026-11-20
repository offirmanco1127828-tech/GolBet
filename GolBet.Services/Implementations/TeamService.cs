using AutoMapper;
using GolBet.Entities;
using GolBet.Repositories.Interfaces;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;

namespace GolBet.Services.Implementations;
public class TeamService : ITeamService
{
    private readonly IGenericRepository<Team> _repository;
    private readonly IMapper _mapper;
    public TeamService(IGenericRepository<Team> repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<TeamDto>> GetAllAsync() => _mapper.Map<IEnumerable<TeamDto>>(await _repository.GetAllAsync());
    public async Task<TeamFormDto?> GetForEditAsync(int id) { var team = await _repository.GetByIdAsync(id); return team is null || !team.IsActive ? null : _mapper.Map<TeamFormDto>(team); }
    public async Task CreateAsync(TeamFormDto dto) { var team = _mapper.Map<Team>(dto); team.IsActive = true; await _repository.AddAsync(team); }
    public async Task UpdateAsync(TeamFormDto dto) { var team = await _repository.GetByIdAsync(dto.Id); if (team is null || !team.IsActive) throw new InvalidOperationException("No se encontró el equipo activo."); _mapper.Map(dto, team); await _repository.UpdateAsync(team); }
    public async Task DeactivateAsync(int id) { var team = await _repository.GetByIdAsync(id); if (team is null || !team.IsActive) throw new InvalidOperationException("No se encontró el equipo activo."); await _repository.DeactivateAsync(id); }
}
