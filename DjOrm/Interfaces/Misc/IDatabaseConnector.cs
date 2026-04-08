public interface IDatabaseConnector
{
    public Task ExecuteCommand(string command);
    public Task<int> ExecuteCommandReturningId(string command);
    public Task ExecuteCommands(IEnumerable<string> commands);
    public Task<IEnumerable<object>> GetDataReaderResults(string command, int propertiesCount);
}