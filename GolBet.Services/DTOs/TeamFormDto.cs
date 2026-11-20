using System.ComponentModel.DataAnnotations;
namespace GolBet.Services.DTOs;
public class TeamFormDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del equipo es obligatorio.")][StringLength(80, ErrorMessage = "El nombre no puede superar 80 caracteres.")] public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "La ciudad es obligatoria.")][StringLength(60, ErrorMessage = "La ciudad no puede superar 60 caracteres.")] public string City { get; set; } = string.Empty;
    [Url(ErrorMessage = "Ingresa una URL válida para el escudo.")][StringLength(300)] public string? CrestUrl { get; set; }
public string PrimaryColor { get; set; } = "#007A33";
}
