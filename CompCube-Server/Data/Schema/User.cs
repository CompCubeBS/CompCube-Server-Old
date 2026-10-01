namespace CompCube_Server.Data.Schema;

public class User
{
    public string Guid { get; set; }
    
    public string BeatKhanaGuid { get; set; }
    public string PlatformId { get; set; }
    
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    
    public bool Banned { get; set; }
    
    public UserFlair? UserFlair { get; set; }
    
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}