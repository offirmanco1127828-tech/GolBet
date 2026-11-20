using Microsoft.AspNetCore.Identity;

namespace GolBet.Entities;

/// <summary>Identity account extended with the player's display name and virtual currency balance.</summary>
public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 100000m;
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
