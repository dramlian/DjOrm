namespace DjOrmTests;

public class FakeDatabaseConnector : IDatabaseConnector
{
    public string LastCommand { get; private set; } = string.Empty;
    public List<string> AllCommands { get; } = new();

    private readonly Queue<IEnumerable<object>> _queuedResults = new();

    public void EnqueueResult(IEnumerable<object> result) => _queuedResults.Enqueue(result);

    public Task ExecuteCommand(string command)
    {
        LastCommand = command;
        AllCommands.Add(command);
        return Task.CompletedTask;
    }

    public Task<int> ExecuteCommandReturningId(string command)
    {
        LastCommand = command;
        AllCommands.Add(command);
        return Task.FromResult(0);
    }

    public Task ExecuteCommands(IEnumerable<string> commands) => Task.CompletedTask;
    public Task<IEnumerable<object>> GetDataReaderResults(string command, int propertiesCount)
    {
        LastCommand = command;
        AllCommands.Add(command);
        if (_queuedResults.Count > 0)
            return Task.FromResult(_queuedResults.Dequeue());
        return Task.FromResult(Enumerable.Empty<object>());
    }
}