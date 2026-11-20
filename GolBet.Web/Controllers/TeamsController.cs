using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GolBet.Web.Controllers;
[Authorize(Roles = "Admin")]
public class TeamsController : Controller
{
    private readonly ITeamService _teamService;
    public TeamsController(ITeamService teamService) => _teamService = teamService;
    [AllowAnonymous] public async Task<IActionResult> Index() => View(await _teamService.GetAllAsync());
    [HttpGet] public IActionResult Create() => View(new TeamFormDto());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Create(TeamFormDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try { await _teamService.CreateAsync(dto); TempData["Success"] = "Equipo creado correctamente."; return RedirectToAction(nameof(Index)); }
        catch (Exception ex) when (ex is InvalidOperationException || ex is Microsoft.EntityFrameworkCore.DbUpdateException) { ModelState.AddModelError(string.Empty, ex is Microsoft.EntityFrameworkCore.DbUpdateException ? "Ya existe un equipo con ese nombre." : ex.Message); return View(dto); }
    }
    [HttpGet] public async Task<IActionResult> Edit(int id) { var dto = await _teamService.GetForEditAsync(id); return dto is null ? NotFound() : View(dto); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Edit(TeamFormDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        try { await _teamService.UpdateAsync(dto); TempData["Success"] = "Equipo actualizado correctamente."; return RedirectToAction(nameof(Index)); }
        catch (Exception ex) when (ex is InvalidOperationException || ex is Microsoft.EntityFrameworkCore.DbUpdateException) { ModelState.AddModelError(string.Empty, ex is Microsoft.EntityFrameworkCore.DbUpdateException ? "Ya existe un equipo con ese nombre." : ex.Message); return View(dto); }
    }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Deactivate(int id) { try { await _teamService.DeactivateAsync(id); TempData["Success"] = "Equipo desactivado correctamente."; } catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; } return RedirectToAction(nameof(Index)); }
}
