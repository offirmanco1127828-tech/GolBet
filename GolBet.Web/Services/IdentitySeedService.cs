using GolBet.Entities;
using Microsoft.AspNetCore.Identity;
namespace GolBet.Web.Services;
public static class IdentitySeedService
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Apostador", "Admin" })
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));

        var email = configuration["AdminBootstrap:Email"];
        var password = configuration["AdminBootstrap:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var admin = await users.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Administrador GolBet",
                Balance = 100000m
            };
            var created = await users.CreateAsync(admin, password);
            if (!created.Succeeded)
                throw new InvalidOperationException("No se pudo crear el administrador inicial: " + string.Join("; ", created.Errors.Select(e => e.Description)));
        }
        else
        {
            var token = await users.GeneratePasswordResetTokenAsync(admin);
            var reset = await users.ResetPasswordAsync(admin, token, password);
            if (!reset.Succeeded)
                throw new InvalidOperationException("No se pudo actualizar la contraseña del admin: " + string.Join("; ", reset.Errors.Select(e => e.Description)));
        }

        if (!await users.IsInRoleAsync(admin, "Admin"))
            await users.AddToRoleAsync(admin, "Admin");
    }
}