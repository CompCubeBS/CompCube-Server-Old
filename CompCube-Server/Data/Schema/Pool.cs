namespace CompCube_Server.Data.Schema;

public class Pool
{
    public string Guid { get; set; }
    
    public int Id { get; set; }

    public List<Beatmap> Maps { get; set; } = [];
}