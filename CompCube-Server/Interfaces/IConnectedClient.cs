using System.Runtime.CompilerServices;
using CompCube.Models;

namespace CompCube_Server.Interfaces;

public interface IConnectedClient
{
    public event Action<PlayerDiscardedMapsMessage, IConnectedClient>? OnUserDiscardedMaps;
    public event Action<PlayerSelectedMapMessage, IConnectedClient>? OnMapSelection;
    public event Action<ScoreSubmission, IConnectedClient>? OnScoreSubmission;
    
    public event Action<IConnectedClient>? OnDisconnected;

    public bool IsConnectionAlive { get; }

    public UserInfo UserInfo { get; }

    public Task SendPacket(ServerPacket packet);

    public Task Disconnect();

    public Task DisconnectAbruptlyAsync(string reason);
}