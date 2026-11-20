using GolBet.Entities.Enums;
using GolBet.Services.DTOs;
using GolBet.Services.Helpers;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GolBet.Web.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchService _matchService;
    private readonly ITeamService _teamService;

    public MatchesController(IMatchService matchService, ITeamService teamService)
    {
        _matchService = matchService;
        _teamService = teamService;
    }

    public async Task<IActionResult> Index(MatchStatus? status)
    {
        ViewBag.CurrentStatus = status;
        return View(await _matchService.GetBoardAsync(status));
    }

    public async Task<IActionResult> Detail(int id)
    {
        var match = await _matchService.GetDetailAsync(id);
        return match is null ? NotFound() : View(match);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var dto = new MatchFormDto { Date = DateTime.Now.AddDays(7).Date.AddHours(19) };
        await LoadTeamsAsync();
        return View(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MatchFormDto dto)
    {
        if (!ModelState.IsValid) { await LoadTeamsAsync(); return View(dto); }
        try
        {
            await _matchService.CreateAsync(dto);
            TempData["Success"] = "Partido creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadTeamsAsync();
            return View(dto);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _matchService.GetForEditAsync(id);
        if (dto is null) return NotFound();
        dto.Date = dto.Date.ToColombiaTime();
        await LoadTeamsAsync();
        return View(dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MatchFormDto dto)
    {
        if (!ModelState.IsValid) { await LoadTeamsAsync(); return View(dto); }
        try
        {
            await _matchService.UpdateAsync(dto);
            TempData["Success"] = "Partido actualizado correctamente.";
            return RedirectToAction(nameof(Detail), new { id = dto.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadTeamsAsync();
            return View(dto);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        try
        {
            await _matchService.DeactivateAsync(id);
            TempData["Success"] = "Partido desactivado correctamente.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(int id)
    {
        try
        {
            await _matchService.StartMatchAsync(id);
            TempData["Success"] = "Partido iniciado. Ya está «En juego».";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Detail), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Close(int id)
    {
        var match = await _matchService.GetDetailAsync(id);
        if (match is null) return NotFound();

        if (match.Status != MatchStatus.InProgress)
        {
            TempData["Error"] = "Solo se puede cerrar un partido que esté «En juego».";
            return RedirectToAction(nameof(Detail), new { id });
        }

        ViewBag.Match = match;
        return View(new MatchResultFormDto { MatchId = id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(MatchResultFormDto dto)
    {
        var match = await _matchService.GetDetailAsync(dto.MatchId);
        if (match is null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.Match = match;
            return View(dto);
        }

        try
        {
            var summary = await _matchService.CloseMatchAsync(dto);
            TempData["Success"] =
                $"Partido cerrado ({dto.HomeGoals} - {dto.AwayGoals}). " +
                $"Apuestas resueltas: {summary.ResolvedBets} | Ganadas: {summary.WonBets} | Total pagado: {summary.TotalPaid:C0}.";
            return RedirectToAction(nameof(Detail), new { id = dto.MatchId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Detail), new { id = dto.MatchId });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> FinishMatch(int id, int homeGoals, int awayGoals)
    {
        await _matchService.CloseMatchAndResolveBetsAsync(id, homeGoals, awayGoals);
        TempData["Success"] = "Partido finalizado y apuestas resueltas.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadTeamsAsync()
    {
        var teams = await _teamService.GetAllAsync();
        ViewBag.Teams = new SelectList(teams, "Id", "Name");
    }
}