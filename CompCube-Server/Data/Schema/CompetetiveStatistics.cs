namespace CompCube_Server.Data.Schema;

public class CompetetiveStatistics
{
    public string Guid { get; set; }
    public User User { get; set; }

    public int Season { get; set; }

    public int CurrentElo { get; set; } = 1000;
    public int Wins { get; set; } = 0;
    public int TotalGamesPlayed { get; set; } = 0;
    public int WinStreak { get; set; } = 0;
    public int BestWinStreak { get; set; } = 0;
}