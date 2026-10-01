namespace CompCube_Server.Data.Schema;

public class CompetetiveStatistics
{
    public required string Guid { get; set; }
    public required User User { get; set; }
    public required string UserGuid { get; set; }

    public required int Season { get; set; }

    public required int Elo { get; set; } = 1000;
    public required int Wins { get; set; } = 0;
    public required int TotalGamesPlayed { get; set; } = 0;
    public required int WinStreak { get; set; } = 0;
    public required int BestWinStreak { get; set; } = 0;
}