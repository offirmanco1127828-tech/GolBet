using GolBet.Entities;
using GolBet.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Bet> Bets => Set<Bet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Team>().Property(t => t.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Team>().HasIndex(t => t.Name).IsUnique();
        modelBuilder.Entity<Match>().HasOne(m => m.HomeTeam).WithMany().HasForeignKey(m => m.HomeTeamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Match>().HasOne(m => m.AwayTeam).WithMany().HasForeignKey(m => m.AwayTeamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Bet>().HasOne(b => b.Match).WithMany(m => m.Bets).HasForeignKey(b => b.MatchId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Bet>().HasOne(b => b.User).WithMany(u => u.Bets).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Bet>().HasIndex(b => new { b.MatchId, b.UserId, b.Pick }).IsUnique();
        modelBuilder.Entity<AppUser>().Property(u => u.Balance).HasColumnType("decimal(12,2)");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added) entry.Entity.CreatedDate = utcNow;
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedDate = utcNow;
                entry.Property(e => e.CreatedDate).IsModified = false;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
