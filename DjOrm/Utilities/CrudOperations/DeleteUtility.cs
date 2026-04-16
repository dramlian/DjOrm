public class DeleteUtility<T> : Utility, IDeleteUtility<T>
{
    public DeleteUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {

    }

    public async Task DeleteData(T input)
    {
        if (input == null) return;
        var pk = GetNameValueOfProperty(input, new List<Type>() { typeof(PrimaryKeyAttribute) })?.FirstOrDefault();
        if (pk is null || pk.Value.Item2 is null)
            throw new InvalidOperationException($"Entity {GetObjName(input)} has no primary key defined.");
        var command = $"DELETE FROM {GetObjName(input)} WHERE {pk.Value.Item1} = {AppendQuotes(pk.Value.Item2, pk.Value.Item2.GetType())};";
        await _dbConnect.ExecuteCommand(command);
    }
}