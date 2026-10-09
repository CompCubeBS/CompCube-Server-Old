using System.ComponentModel.DataAnnotations;

namespace CompCube_Server.Data.Schema;

public class MatchResult
{
    [Key]
    [MaxLength(36)]
    public required string Guid { get; set; }
    
    [Required]
    [MaxLength(36)]
    public string WinnerGuid { get; set; }
    
    [Required]
    public User Winner { get; set; }
    
    [Required]
    [MaxLength(36)]
    public string LoserGuid { get; set; }
    
    [Required]
    public User Loser { get; set; }
    
    [Required]
    [MaxLength(36)]
    public string FirstPickerGuid { get; set; }
    
    [Required]
    public User FirstPicker { get; set; }

    [Required]
    public List<Score> Scores { get; set; } = [];

    [Required]
    public int EloTransfer { get; set; } = 0;

    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
}