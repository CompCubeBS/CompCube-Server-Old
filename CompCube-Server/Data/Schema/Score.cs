using System.ComponentModel.DataAnnotations;

namespace CompCube_Server.Data.Schema;

public class Score
{
    [Key]
    [MaxLength(36)]
    public required string Guid { get; set; }
    
    [Required]
    [MaxLength(36)]
    public string BeatmapGuid { get; set; }
    
    [Required]
    public Beatmap Beatmap { get; set; }
    
    [Required]
    public required int Points { get; set; }
    
    [Required]
    public required int Misses { get; set; }
    
    [Required]
    public required bool FullCombo { get; set; }
    
    [Required]
    [MaxLength(36)]
    public string OwnerGuid { get; set; }
    
    [Required]
    public User Owner { get; set; }
    
    public string? MatchResultGuid { get; set; }
    
    public MatchResult? MatchResult { get; set; }
}