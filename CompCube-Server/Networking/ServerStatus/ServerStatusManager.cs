using CompCube.Models;

namespace CompCube_Server.Networking.ServerStatus;

public class ServerStatusManager(IConfiguration config)
{
    private ServerState _state = ServerState.Online;

    public ServerState State
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChanged?.Invoke(value);
        }
    }
    
    public event Action<ServerState>? OnStateChanged;

    public CompCube.Models.ServerStatus GetServerStatus()
    {
        var serverSection = config.GetSection("Gameplay");

        var allowedGameVersions = serverSection.GetSection("AllowedGameVersions").Get<string[]>();

        if (allowedGameVersions == null)
            throw new Exception("Could not parse allowed game versions!");
        
        var allowedModVersions = serverSection.GetSection("AllowedModVersions").Get<string[]>();
        
        if (allowedModVersions == null)
            throw new Exception("Could not parse allowed mod versions!");
        
        return new CompCube.Models.ServerStatus(allowedGameVersions, allowedModVersions, State);
    }
}