using GolBet.Entities;
using GolBet.Entities.Enums;
using GolBet.Repositories.Data;
using GolBet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Implementations;

public class BetRepository : GenericRepository<Bet>, IBetRepository
{
    public BetRepository(AppDbContext context) : base(context) { }

    public async Task<List<Bet>> GetPendingByMatchAsync(int matchId)
        => await _dbSet
            .Where(b => b.MatchId == matchId && b.Status == BetStatus.Pending)
            .Include(b => b.User)
            .ToListAsync();
}