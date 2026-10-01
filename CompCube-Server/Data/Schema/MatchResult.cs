namespace CompCube_Server.Data.Schema;

public class MatchResult
{
    public string Guid { get; set; }
    
    public required string WinnerGuid { get; set; }
    public required User Winner { get; set; }
    
    public required string LoserGuid { get; set; }
    public required User Loser { get; set; }
    
    public required string FirstPickerGuid { get; set; }
    public required User FirstPicker { get; set; }

    public List<Score> Scores { get; set; } = [];

    public required int EloTransfer { get; set; } = 0;

    public required DateTime EndDate { get; set; } = DateTime.UtcNow;
}