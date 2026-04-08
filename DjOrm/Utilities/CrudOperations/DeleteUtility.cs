public class DeleteUtility<T> : Utility, IDeleteUtility<T>
{
    public DeleteUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {

    }

    public async Task DeleteData(T input)
    {
        if (input == null) return;
        var pk = (await GetNameValueOfProperty(input, new List<Type>() { typeof(PrimaryKeyAttribute) }))?.FirstOrDefault();
        if (pk is null) return;
        var command = $"DELETE FROM {GetObjName(input)} WHERE {pk.Value.Item1} = {AppendQuotes(pk.Value.Item2, pk.Value.Item2.GetType())};";
        await _dbConnect.ExecuteCommand(command);
    }
}