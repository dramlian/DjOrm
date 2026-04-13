namespace DjOrmTests;

public class FakeDatabaseConnector : IDatabaseConnector
{
    public string LastCommand { get; private set; } = string.Empty;

    public Task ExecuteCommand(string command)
    {
        LastCommand = command;
        return Task.CompletedTask;
    }

    public Task<int> ExecuteCommandReturningId(string command) => Task.FromResult(0);
    public Task ExecuteCommands(IEnumerable<string> commands) => Task.CompletedTask;
    public Task<IEnumerable<object>> GetDataReaderResults(string command, int propertiesCount) =>
        Task.FromResult(Enumerable.Empty<object>());
}