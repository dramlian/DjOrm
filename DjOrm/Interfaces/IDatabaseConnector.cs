public interface IDatabaseConnector
{
    public void ExecuteCommands(IEnumerable<string> commands);
}