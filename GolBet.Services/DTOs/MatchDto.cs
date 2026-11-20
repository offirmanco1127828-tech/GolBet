using GolBet.Entities.Enums;

namespace GolBet.Services.DTOs;

public class MatchDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public MatchStatus Status { get; set; }
    public string HomeTeamName { get; set; } = null!;
    public string? HomeTeamCrestUrl { get; set; }
    public string HomeTeamColor { get; set; } = "#007A33";
    public string AwayTeamName { get; set; } = null!;
    public string? AwayTeamCrestUrl { get; set; }
    public string AwayTeamColor { get; set; } = "#1D4ED8";
    public int? HomeGoals { get; set; }
    public int? AwayGoals { get; set; }
    public decimal HomeOdds { get; set; }
    public decimal DrawOdds { get; set; }
    public decimal AwayOdds { get; set; }
}