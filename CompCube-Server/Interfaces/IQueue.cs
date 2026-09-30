namespace CompCube_Server.Interfaces;

public interface IQueue
{
    public string QueueName { get; }
    
    public void AddClientToPool(IConnectedClient client);
}