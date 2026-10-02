namespace CompCube_Server.Data.Schema;

public class User
{
    public string Guid { get; set; }
    
    public string? BeatKhanaGuid { get; set; }
    public required string PlatformId { get; set; }
    
    public required string Username { get; set; }
    public required string AvatarUrl { get; set; }

    public required bool Banned { get; set; } = false;
    
    public string? FlairGuid { get; set; }
    
    public UserFlair? Flair { get; set; }
    
    public List<Score> Scores { get; set; } = [];
    
    public List<CompetetiveStatistics> CompetetiveStatistics { get; set; } = [];
    
    public required DateTime Created { get; set; } = DateTime.UtcNow;
    public required DateTime Updated { get; set; } = DateTime.UtcNow;
}