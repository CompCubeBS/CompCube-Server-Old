using CompCube_Models.Models.Map;
using CompCube_Server.Data.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompCube_Server.Data;

public class CompCubeDbContext(DbContextOptions<CompCubeDbContext> options) : DbContext(options)
{
    public DbSet<Beatmap> Beatmaps => Set<Beatmap>();
    public DbSet<CompetetiveStatistics> CompetetiveStatistics => Set<CompetetiveStatistics>();
    public DbSet<MatchResults> MatchResults => Set<MatchResults>();
    public DbSet<Pool> Pools => Set<Pool>();
    public DbSet<Queue> Queues => Set<Queue>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Score> Scores => Set<Score>();
    public DbSet<UserFlair> UserFlairs => Set<UserFlair>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beatmap>(entity =>
        {
            entity.ToTable("beatmaps");

            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");

            entity.Property(p => p.Hash).HasMaxLength(64).IsRequired();
            entity.Property(p => p.MaxScore).IsRequired();
            entity.Property(p => p.DurationSeconds).IsRequired();
            entity.Property(p => p.Category).IsRequired();
            entity.Property(p => p.Difficulty).IsRequired();
        });
    }
    
}