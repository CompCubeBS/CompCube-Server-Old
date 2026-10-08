using System.ComponentModel.DataAnnotations;
using CompCube_Models.Models.Map;

namespace CompCube_Server.Data.Schema;

public class Beatmap
{
    [Key]
    [MaxLength(36)]
    public required string Guid { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string Hash { get; set; }
    
    [Required]
    public int MaxScore { get; set; }
    
    [Required]
    public VotingMap.Category Category { get; set; }
    
    [Required]
    public VotingMap.DifficultyType Difficulty { get; set; }
    
    [Required]
    public int DurationSeconds { get; set; }
    
    [Required]
    public List<Pool> Pools { get; set; } = [];
    
    [Required]
    public List<Score> Scores { get; set; } = [];
}