public interface IDatabaseConnector
{
    public Task ExecuteCommand(string command);
    public Task<int> ExecuteCommandReturningId(string command);
}