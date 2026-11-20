namespace GolBet.Services.DTOs;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? CrestUrl { get; set; }
    public string PrimaryColor { get; set; } = "#007A33";
    public bool IsActive { get; set; }
}