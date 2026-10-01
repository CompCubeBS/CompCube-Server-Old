namespace CompCube_Server.Data.Schema;

public class Queue
{
    public string Guid { get; set; }
    
    public required string Name { get; set; }
    public required string Slug { get; set; }
    
    public required List<Pool> ActivePools { get; set; } = [];

    public required bool UsesMatchmaking { get; set; } = true;
    public required bool Competitive { get; set; } = true;

    public required bool Enabled { get; set; } = true;
}