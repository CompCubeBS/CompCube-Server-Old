namespace CompCube_Server.Data.Schema;

public class Score
{
    public string Guid { get; set; }
    
    public Beatmap Beatmap { get; set; }
    
    public int Points { get; set; }
    
    public int Misses { get; set; }
    
    public bool FullCombo { get; set; }
    
    public User Owner { get; set; }
}