using CompCube_Models.Models.ClientData;
using CompCube_Server.Config;
using CompCube_Server.Data.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompCube_Server.Data;

public class UserData(DataContext context, ConfigHelper configHelper)
{
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

        var rank = context.CompetetiveStatistics.Where(i => i.Season == season).Count(i => i.Elo > stats.Elo) + 1;
        
        return new UserStatistics(user.Username, user.PlatformId, user.BeatKhanaGuid, user.AvatarUrl, GetFlairFromModel(user.Flair), user.Banned, rank, stats.Elo, stats.Wins, stats.TotalGamesPlayed, stats.WinStreak, stats.BestWinStreak);
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