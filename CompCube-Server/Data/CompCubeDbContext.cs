using CompCube_Models.Models.Map;
using CompCube_Server.Data.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompCube_Server.Data;

public class CompCubeDbContext(DbContextOptions<CompCubeDbContext> options) : DbContext(options)
{
    public DbSet<Beatmap> Beatmaps => Set<Beatmap>();
    public DbSet<CompetetiveStatistics> CompetetiveStatistics => Set<CompetetiveStatistics>();
    public DbSet<MatchResult> MatchResults => Set<MatchResult>();
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");

            entity.Property(p => p.Username).IsRequired();
            entity.Property(p => p.AvatarUrl)
                .HasDefaultValue("https://cdn.scoresaber.com/avatars/oculus.png?v=1781213201");

            entity.Property(p => p.Banned).IsRequired().HasDefaultValue(false);

            entity.Property(p => p.BeatKhanaGuid).IsRequired();
            entity.HasIndex(p => p.BeatKhanaGuid).IsUnique();

            entity.Property(p => p.PlatformId).IsRequired();
            entity.HasIndex(p => p.PlatformId).IsUnique();

            entity.Property(p => p.FlairGuid).HasDefaultValue(null);
            entity.HasOne(p => p.Flair)
                .WithMany()
                .HasForeignKey(u => u.FlairGuid)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(p => p.Created).IsRequired();
            entity.Property(p => p.Updated).IsRequired();
        });

        modelBuilder.Entity<Pool>(entity =>
        { 
            entity.ToTable("pools");
            
            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");
            
            entity.Property(p => p.Id).IsRequired();
            entity.HasIndex(p => p.Id).IsUnique();
        });

        modelBuilder.Entity<Queue>(entity =>
        {
            entity.ToTable("queues");
            
            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");
            
            entity.HasIndex(p => p.Slug).IsUnique();
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.ToTable("scores");
            
            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");

            entity.HasOne(p => p.Beatmap)
                .WithMany(p => p.Scores)
                .HasForeignKey(p => p.BeatmapGuid)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(p => p.Owner)
                .WithMany(p => p.Scores)
                .HasForeignKey(p => p.OwnerGuid)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CompetetiveStatistics>(entity =>
        {
            entity.ToTable("competetive_statistics");
            
            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");
            
            entity.HasOne(p => p.User)
                .WithMany(p => p.CompetetiveStatistics)
                .HasForeignKey(p => p.UserGuid)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MatchResult>(entity =>
        {
            entity.ToTable("match_results");
            
            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");
            
            entity.HasMany(p => p.Scores)
                .WithOne(p => p.MatchResult)
                .HasForeignKey(p => p.MatchResultGuid)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(p => p.Winner)
                .WithMany()
                .HasForeignKey(p => p.WinnerGuid)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(p => p.Loser)
                .WithMany()
                .HasForeignKey(p => p.LoserGuid)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(p => p.FirstPicker)
                .WithMany()
                .HasForeignKey(p => p.FirstPickerGuid)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UserFlair>(entity =>
        {
            entity.ToTable("user_flairs");
            
            entity.HasKey(p => p.Guid);
            entity.Property(p => p.Guid).HasDefaultValueSql("NEWID()");
        });
    }
}