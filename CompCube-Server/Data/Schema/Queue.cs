namespace CompCube_Server.Data.Schema;

public class Queue
{
    public string Guid { get; set; }
    
    public string Name { get; set; }
    public string Slug { get; set; }
    
    public List<Pool> ActivePools { get; set; } = [];

    public bool UsesMatchmaking { get; set; } = true;
    public bool Competitive { get; set; } = true;

    public bool Enabled { get; set; } = true;
}