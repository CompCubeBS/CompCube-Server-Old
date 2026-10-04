using CompCube_Models.Models.Map;
using CompCube_Server.Config;
using Microsoft.EntityFrameworkCore;

namespace CompCube_Server.Data;

public class MapData(DataContext context, ConfigHelper helper)
{
    public List<VotingMap> GetAllMapsFromActiveBatches()
    {
        var pools = context.Pools.Include(p => p.Maps).Where(i => helper.ActivePools.Contains(i.Id));
        
        var maps = new List<VotingMap>();

        foreach (var pool in pools)
        {
            maps = maps.Concat(pool.Maps.Select(i => new VotingMap(i.Hash, i.Difficulty, i.Category))).ToList();
        }

        return maps;
    }
}