using System.ComponentModel.DataAnnotations;
namespace GolBet.Services.DTOs;
public class MatchFormDto
{
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona el equipo local.")] public int HomeTeamId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona el equipo visitante.")] public int AwayTeamId { get; set; }
    [Required(ErrorMessage = "La fecha del partido es obligatoria.")] public DateTime Date { get; set; } = DateTime.Today.AddDays(7).AddHours(19);
    [Range(typeof(decimal), "1.01", "999.99", ErrorMessage = "La cuota debe ser mayor que 1.00.")] public decimal HomeOdds { get; set; } = 2.00m;
    [Range(typeof(decimal), "1.01", "999.99", ErrorMessage = "La cuota debe ser mayor que 1.00.")] public decimal DrawOdds { get; set; } = 3.00m;
    [Range(typeof(decimal), "1.01", "999.99", ErrorMessage = "La cuota debe ser mayor que 1.00.")] public decimal AwayOdds { get; set; } = 3.50m;
}
