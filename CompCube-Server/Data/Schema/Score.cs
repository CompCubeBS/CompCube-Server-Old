namespace CompCube_Server.Data.Schema;

public class Score
{
    public required string Guid { get; set; }
    
    public required string BeatmapGuid { get; set; }
    
    public required Beatmap Beatmap { get; set; }
    
    public required int Points { get; set; }
    
    public required int Misses { get; set; }
    
    public required bool FullCombo { get; set; }
    
    public required string OwnerGuid { get; set; }
    
    public required User Owner { get; set; }
    
    public required string MatchResultGuid { get; set; }
    
    public required MatchResult MatchResult { get; set; }
}