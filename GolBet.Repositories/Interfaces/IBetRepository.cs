using System.Collections.Generic;
using System.Threading.Tasks;
using GolBet.Entities; // <-- Cambia este namespace por el que use tu clase Bet

namespace GolBet.Repositories.Interfaces;

public interface IBetRepository
{
    /// <summary>Pending bets of a match, TRACKED and with their users loaded</summary>
    Task<List<Bet>> GetPendingByMatchAsync(int matchId);
}