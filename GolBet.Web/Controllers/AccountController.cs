using GolBet.Entities;
using GolBet.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) { _userManager = userManager; _signInManager = signInManager; }
    [HttpGet] public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View();
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new AppUser { UserName = model.Email.Trim(), Email = model.Email.Trim(), FullName = model.FullName.Trim(), Balance = 100000m, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Apostador");
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = "¡Cuenta creada! Recibiste 100.000 FutCoins de bienvenida.";
            return RedirectToAction("Index", "Home");
        }
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, TranslateIdentityError(error.Code));
        return View(model);
    }
    [HttpGet] public IActionResult Login(string? returnUrl = null) { if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home"); return View(new LoginViewModel { ReturnUrl = returnUrl }); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await _signInManager.PasswordSignInAsync(model.Email.Trim(), model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded) return LocalRedirect(string.IsNullOrWhiteSpace(model.ReturnUrl) ? Url.Action("Index", "Home")! : model.ReturnUrl);
        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos."); return View(model);
    }
    [HttpGet] public IActionResult AccessDenied() => View();
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Logout() { await _signInManager.SignOutAsync(); return RedirectToAction("Index", "Home"); }
    private static string TranslateIdentityError(string code) => code switch
    {
        "DuplicateUserName" or "DuplicateEmail" => "Ya existe una cuenta con ese correo.",
        "PasswordTooShort" => "La contraseña debe tener al menos 8 caracteres.",
        "PasswordRequiresDigit" => "La contraseña debe incluir al menos un número.",
        "PasswordRequiresUpper" => "La contraseña debe incluir al menos una mayúscula.",
        "PasswordRequiresLower" => "La contraseña debe incluir al menos una minúscula.",
        "PasswordRequiresNonAlphanumeric" => "La contraseña debe incluir al menos un símbolo.",
        _ => "No fue posible crear la cuenta. Revisa los datos ingresados."
    };
}
