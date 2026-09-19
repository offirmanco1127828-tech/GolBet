using GolBet.Entities;
using GolBet.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Teams.AnyAsync())
            return;

        var teams = new[]
        {
            new Team { Name = "Atlético Nacional", City = "Medellín", CrestUrl = "https://placehold.co/100x100?text=AN" },
            new Team { Name = "Independiente Medellín", City = "Medellín", CrestUrl = "https://placehold.co/100x100?text=DIM" },
            new Team { Name = "Millonarios", City = "Bogotá", CrestUrl = "https://placehold.co/100x100?text=MIL" },
            new Team { Name = "Independiente Santa Fe", City = "Bogotá", CrestUrl = "https://placehold.co/100x100?text=SF" },
            new Team { Name = "América de Cali", City = "Cali", CrestUrl = "https://placehold.co/100x100?text=AME" },
            new Team { Name = "Deportivo Cali", City = "Cali", CrestUrl = "https://placehold.co/100x100?text=CAL" },
            new Team { Name = "Junior de Barranquilla", City = "Barranquilla", CrestUrl = "https://placehold.co/100x100?text=JUN" },
            new Team { Name = "Once Caldas", City = "Manizales", CrestUrl = "https://placehold.co/100x100?text=ONC" }
        };

        context.Teams.AddRange(teams);
        await context.SaveChangesAsync();

        var matches = new[]
        {
            new Match { Date = DateTime.UtcNow.AddDays(1), Status = MatchStatus.Scheduled, HomeTeamId = teams[0].Id, AwayTeamId = teams[2].Id, HomeOdds = 1.80m, DrawOdds = 3.40m, AwayOdds = 4.20m },
            new Match { Date = DateTime.UtcNow.AddDays(1).AddHours(3), Status = MatchStatus.Scheduled, HomeTeamId = teams[1].Id, AwayTeamId = teams[3].Id, HomeOdds = 2.10m, DrawOdds = 3.10m, AwayOdds = 3.60m },
            new Match { Date = DateTime.UtcNow.AddDays(2), Status = MatchStatus.Scheduled, HomeTeamId = teams[4].Id, AwayTeamId = teams[5].Id, HomeOdds = 2.00m, DrawOdds = 3.20m, AwayOdds = 3.80m },
            new Match { Date = DateTime.UtcNow.AddDays(2).AddHours(4), Status = MatchStatus.Scheduled, HomeTeamId = teams[6].Id, AwayTeamId = teams[7].Id, HomeOdds = 1.95m, DrawOdds = 3.30m, AwayOdds = 3.90m },
            new Match { Date = DateTime.UtcNow.AddHours(-1), Status = MatchStatus.InProgress, HomeTeamId = teams[2].Id, AwayTeamId = teams[4].Id, HomeOdds = 2.30m, DrawOdds = 3.00m, AwayOdds = 3.10m },
            new Match { Date = DateTime.UtcNow.AddDays(-1), Status = MatchStatus.Finished, HomeGoals = 2, AwayGoals = 1, HomeTeamId = teams[0].Id, AwayTeamId = teams[1].Id, HomeOdds = 1.70m, DrawOdds = 3.50m, AwayOdds = 4.50m }
        };

        context.Matches.AddRange(matches);
        await context.SaveChangesAsync();
    }
}
