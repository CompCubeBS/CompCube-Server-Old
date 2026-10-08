using System.ComponentModel.DataAnnotations;

namespace CompCube_Server.Data.Schema;

public class CompetetiveStatistics
{
    [MaxLength(36)]
    [Key]
    public string Guid { get; set; }
    
    [Required]
    public User User { get; set; }
    
    [Required]
    [MaxLength(36)]
    public string UserGuid { get; set; }

    [Required]
    public int Season { get; set; }

    [Required]
    public int Elo { get; set; } = 1000;
    
    [Required]
    public int Wins { get; set; } = 0;
    
    [Required]
    public int TotalGamesPlayed { get; set; } = 0;
    
    [Required]
    public int WinStreak { get; set; } = 0;
    
    [Required]
    public int BestWinStreak { get; set; } = 0;
}