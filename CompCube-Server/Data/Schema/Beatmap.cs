using CompCube_Models.Models.Map;

namespace CompCube_Server.Data.Schema;

public class Beatmap
{
    public string Guid { get; set; }
    public string Hash { get; set; }
    public int MaxScore { get; set; }
    public VotingMap.Category Category { get; set; }
    public VotingMap.DifficultyType Difficulty { get; set; }
    public int DurationSeconds { get; set; }
}