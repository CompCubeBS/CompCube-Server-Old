namespace CompCube_Server.Data.Schema;

public class MatchResults
{
    public string Guid { get; set; }
    
    public User Winner { get; set; }
    public User Loser { get; set; }
    
    public User FirstPicker { get; set; }

    public List<Score> Scores { get; set; } = [];

    public int EloTransfer { get; set; } = 0;

    public DateTime EndDate { get; set; }
}