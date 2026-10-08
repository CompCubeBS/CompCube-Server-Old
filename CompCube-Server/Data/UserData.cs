using CompCube_Models.Models.ClientData;
using CompCube_Server.Config;
using CompCube_Server.Data.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CompCube_Server.Data;

public class UserData(DataContext context, ConfigHelper configHelper, Logger<UserData> logger)
{
    public UserStatistics Debug => new UserStatistics("debug", 
        "0", 
        null, 
        "https://cdn.scoresaber.com/avatars/oculus.png?v=1781213201",
        null,
        false,
        0,
        1000,
        0,
        0,
        0,
        0
        );

    public List<UserStatistics> GetLeaderboardRange(int start, int range, int season = -1)
    {
        if (season == -1)
            season = configHelper.Season;

        return context.Users
            .Include(p => p.CompetetiveStatistics)
            .Include(p => p.Flair)
            .Where(p => p.CompetetiveStatistics.Any(i => i.Season == season))
            .OrderBy(p => p.CompetetiveStatistics.First(i => i.Season == season).Elo)
            .Skip(start - 1)
            .Take(range)
            .ToArray()
            .Select(i =>
            {
                var stats = i.CompetetiveStatistics.First(j => j.Season == season);

                return new UserStatistics(i.Username, 
                    i.PlatformId, 
                    i.BeatKhanaGuid, 
                    i.AvatarUrl, 
                    GetFlairFromModel(i.Flair),
                    i.Banned,
                    GetRankFromElo(stats.Elo, season),
                    stats.Elo,
                    stats.Wins,
                    stats.TotalGamesPlayed,
                    stats.WinStreak,
                    stats.BestWinStreak);
            }).ToList();
    }
    
    public List<UserStatistics>? GetAroundUser(string platformId, int season = -1)
    {
        if (season == -1)
            season = configHelper.Season;

        var users = context.Users
            .Include(p => p.CompetetiveStatistics)
            .Include(p => p.Flair)
            .Where(p => p.CompetetiveStatistics.Any(i => i.Season == season))
            .Where(p => !p.Banned)
            .OrderBy(p => p.CompetetiveStatistics.First(i => i.Season == season).Elo).ToArray();
        
        var index = Array.FindIndex(users, u => u.PlatformId == platformId);

        if (index == -1)
            return null;

        var startIndex = index - 5;
        
        var count = Math.Min(10, users.Length - startIndex);
        
        return users.Skip(startIndex).Take(count).Select(user =>
        {
            var stats = user.CompetetiveStatistics.First(k => k.Season == season);

            return new UserStatistics(user.Username, user.PlatformId, user.BeatKhanaGuid, user.AvatarUrl,
                GetFlairFromModel(user.Flair), user.Banned, GetRankFromElo(stats.Elo, season), stats.Elo, stats.Wins,
                stats.TotalGamesPlayed, stats.WinStreak, stats.BestWinStreak);
        }).ToList();
    }

    public UserStatistics UpdateUserOnLogin(string platformId, string username, string avatarUrl)
    {
        var user = context.Users.Include(p => p.CompetetiveStatistics).FirstOrDefault(p => p.PlatformId == platformId);

        using var transaction = context.Database.BeginTransaction();

        try
        {
            if (user == null)
            {
                context.Users.Add(new User()
                {
                    PlatformId = platformId,
                    Username = username,
                    AvatarUrl = avatarUrl,
                    Banned = false,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                });

                context.SaveChanges();

                user = context.Users.Include(p => p.CompetetiveStatistics).First(p => p.PlatformId == platformId);

                CreateNewCompetitiveStatisticsIfNotExists(user.PlatformId);

                context.SaveChanges();

                transaction.Commit();
                return GetUserStatisticsByPlatformId(platformId)!;
            }

            user.AvatarUrl = avatarUrl;
            user.Username = username;
            
            CreateNewCompetitiveStatisticsIfNotExists(user.PlatformId);

            context.SaveChanges();
            transaction.Commit();

            return GetUserStatisticsByPlatformId(platformId)!;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to create user info for {platformId}");
            throw;
        }
    }

    private void CreateNewCompetitiveStatisticsIfNotExists(string platformId)
    {
        var userToCreateFor = context.Users
            .Include(i => i.CompetetiveStatistics)
            .FirstOrDefault(i => i.PlatformId == platformId);

        if (userToCreateFor == null)
            return;

        if (userToCreateFor.CompetetiveStatistics.Any(i => i.Season == configHelper.Season))
            return;

        context.CompetitiveStatistics.Add(new CompetetiveStatistics()
        {
            BestWinStreak = 0,
            Elo = 1000,
            Season = configHelper.Season,
            TotalGamesPlayed = 0,
            WinStreak = 0,
            Wins = 0,
            User = userToCreateFor
        });

        context.SaveChanges();
    }

    public void IncrementWins(string platformId)
    {
        using var transaction = context.Database.BeginTransaction();

        try
        {
            var user = context.Users
                .Include(i => i.CompetetiveStatistics)
                .FirstOrDefault(i => i.PlatformId == platformId);

            if (user == null)
                return;
            
            var stats = user.CompetetiveStatistics.First(i => i.Season == configHelper.Season);

            stats.WinStreak++;
            context.SaveChanges();

            stats.Wins++;
            context.SaveChanges();
            
            if (stats.WinStreak > stats.BestWinStreak)
            {
                stats.BestWinStreak = stats.WinStreak;
                context.SaveChanges();
            }

            transaction.Commit();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to increment wins for {platformId}");
            throw;
        }
    }

    public void AdjustElo(string platformId, int eloChange)
    {
        using var transaction = context.Database.BeginTransaction();
        
        try
        {
            var user = context.Users
                .Include(i => i.CompetetiveStatistics)
                .FirstOrDefault(i => i.PlatformId == platformId);

            if (user == null)
                return;
            
            var stats = user.CompetetiveStatistics.First(i => i.Season == configHelper.Season);

            stats.Elo += eloChange;
            context.SaveChanges();

            transaction.Commit();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to adjust elo for {platformId}");
            throw;
        }
    }

    public void ResetWinstreak(string platformId)
    {
        using var transaction = context.Database.BeginTransaction();

        try
        {
            var user = context.Users
                .Include(i => i.CompetetiveStatistics)
                .FirstOrDefault(i => i.PlatformId == platformId);

            if (user == null)
                return;
            
            var stats = user.CompetetiveStatistics.First(i => i.Season == configHelper.Season);

            stats.WinStreak = 0;
            context.SaveChanges();

            transaction.Commit();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to reset winstreak for {platformId}");
            throw;
        }
    }

    public void IncrementTotalGames(string platformId)
    {
        using var transaction = context.Database.BeginTransaction();

        try
        {
            var user = context.Users
                .Include(i => i.CompetetiveStatistics)
                .FirstOrDefault(i => i.PlatformId == platformId);

            if (user == null)
                return;

            user.CompetetiveStatistics
                .First(i => i.Season == configHelper.Season)
                .TotalGamesPlayed++;

            context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to increment total games for {platformId}");
            throw;
        }
    }
    
    public List<UserStatistics> GetAllUserStatistics(int season = -1)
    {
        var users = context.Users.Include(p => p.CompetetiveStatistics)
            .Include(p => p.Flair)
            .Where(i => !i.Banned)
            .Where(i => i.CompetetiveStatistics.Any(j => j.Season == season))
            .ToArray();

        return users.Select(user =>
        {
            var stats = user.CompetetiveStatistics.First(k => k.Season == season);

            return new UserStatistics(user.Username, user.PlatformId, user.BeatKhanaGuid, user.AvatarUrl,
                GetFlairFromModel(user.Flair), user.Banned, GetRankFromElo(stats.Elo, season), stats.Elo, stats.Wins,
                stats.TotalGamesPlayed, stats.WinStreak, stats.BestWinStreak);
        }).ToList();
    }
    
    public UserInfo? GetUserInfoByPlatformId(string platformId)
    {
        var user = context.Users.Include(user => user.Flair).FirstOrDefault(i => i.PlatformId == platformId);

        return GetUserInfoFromUserModel(user);
    }

    public UserStatistics? GetUserStatisticsByPlatformId(string platformId, int season = -1)
    {
        if (season == -1)
            season = configHelper.Season;
        
        var user = context.Users.Include(p => p.CompetetiveStatistics).Include(user => user.Flair).FirstOrDefault(i => i.PlatformId == platformId);

        if (user == null)
            return null;
        
        var stats = user.CompetetiveStatistics.FirstOrDefault(i => i.Season == season);

        if (stats == null)
            return null;

        var rank = GetRankFromElo(stats.Elo, season);
        
        return new UserStatistics(user.Username, user.PlatformId, user.BeatKhanaGuid, user.AvatarUrl, GetFlairFromModel(user.Flair), user.Banned, rank, stats.Elo, stats.Wins, stats.TotalGamesPlayed, stats.WinStreak, stats.BestWinStreak);
    }

    private int GetRankFromElo(int elo, int season)
    {
        if (season == -1)
            season = configHelper.Season;
        
        return context.CompetitiveStatistics.Where(i => i.Season == season).Count(i => i.Elo > elo) + 1;
    }

    private Flair? GetFlairFromModel(UserFlair? flair)
    {
        if (flair == null)
            return null;

        return new Flair(flair.Name, flair.Color);
    }

    private UserInfo? GetUserInfoFromUserModel(User? user)
    {
        if (user == null)
            return null;
        
        return new UserInfo(user.Username, user.PlatformId, user.BeatKhanaGuid, user.AvatarUrl, GetFlairFromModel(user.Flair), user.Banned);
    }
}