public class DeleteUtility<T> : Utility, IDeleteUtility<T>
{
    public DeleteUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {

    }

    public async Task DeleteData(T input)
    {
        if (input == null) return;
        var pk = (await GetNameValueOfProperty(input, new List<Type>() { typeof(PrimaryKeyAttribute) })).FirstOrDefault();
        var command = $"DELETE FROM {input?.GetType().FullName} WHERE {pk.Item1} = {AppendQuotes(pk.Item2, pk.Item2.GetType())};";
        await _dbConnect.ExecuteCommand(command);
    }
}