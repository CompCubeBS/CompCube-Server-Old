using CompCube_Models.Models.ClientData;
using CompCube_Server.Config;
using CompCube_Server.Data.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompCube_Server.Data;

public class UserData(DataContext context, ConfigHelper configHelper)
{
    public List<UserStatistics>? GetAroundUser(string platformId, int season = -1)
    {
        if (season == -1)
            season = configHelper.Season;

        var users = context.Users
            .Include(p => p.CompetetiveStatistics)
            .Include(p => p.Flair)
            .Where(p => p.CompetetiveStatistics.Any(i => i.Season == season))
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
                GetFlairFromModel(user.Flair), user.Banned, GetRankFromElo(stats.Elo), stats.Elo, stats.Wins,
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

                CreateNewCompetitiveStatistics(user);

                context.SaveChanges();

                transaction.Commit();
                return GetUserStatisticsByPlatformId(platformId)!;
            }

            user.AvatarUrl = avatarUrl;
            user.Username = username;
            
            CreateNewCompetitiveStatistics(user);

            context.SaveChanges();
            transaction.Commit();

            return GetUserStatisticsByPlatformId(platformId)!;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to create user info for {platformId}: {e.Message}");
        }

        void CreateNewCompetitiveStatistics(User userToCreateFor)
        {
            if (userToCreateFor.CompetetiveStatistics.Any(i => i.Season == configHelper.Season))
                return;
            
            context.CompetetiveStatistics.Add(new CompetetiveStatistics()
            {
                BestWinStreak = 0,
                Elo = 1000,
                Season = configHelper.Season,
                TotalGamesPlayed = 0,
                WinStreak = 0,
                Wins = 0,
                User = user
            });
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
                GetFlairFromModel(user.Flair), user.Banned, GetRankFromElo(stats.Elo), stats.Elo, stats.Wins,
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

        var rank = GetRankFromElo(stats.Elo);
        
        return new UserStatistics(user.Username, user.PlatformId, user.BeatKhanaGuid, user.AvatarUrl, GetFlairFromModel(user.Flair), user.Banned, rank, stats.Elo, stats.Wins, stats.TotalGamesPlayed, stats.WinStreak, stats.BestWinStreak);
    }

    private int GetRankFromElo(int elo, int season = -1)
    {
        if (season == -1)
            season = configHelper.Season;
        
        return context.CompetetiveStatistics.Where(i => i.Season == season).Count(i => i.Elo > elo) + 1;
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