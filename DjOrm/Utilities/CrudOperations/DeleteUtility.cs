public class DeleteUtility<T> : IDeleteUtility<T>
{
    private IDatabaseConnector _dbConnect;

    public DeleteUtility(IDatabaseConnector dbConnect)
    {
        _dbConnect = dbConnect;
    }

    public async Task DeleteData(T input)
    {
        var pk = await GetPrivateKeyNameValue(input);
        var command = $"DELETE FROM {input?.GetType().FullName} WHERE {pk.Item1} = {AppendQuotes(pk.Item2, pk.Item2.GetType())};";
        await _dbConnect.ExecuteCommand(command);
    }

    private async Task<(string, object)> GetPrivateKeyNameValue(T input)
    {
        return input?.GetType()
               .GetProperties()
               .Where(p => p.CustomAttributes
               .Any(a => a.AttributeType == typeof(PrimaryKeyAttribute)))
               .Select(x => (x.Name, GetValueOfProperty(input, x.Name))).FirstOrDefault() ?? throw new Exception("");
    }

    private object GetValueOfProperty(object input, string name)
    {
        return input?.GetType()?.GetProperty(name)?.GetValue(input) ?? throw new Exception("");
    }

    private object AppendQuotes(object obj, Type propertyType)
    {
        if (propertyType == typeof(string))
        {
            return $"'{obj}'";
        }
        else
        {
            return obj;
        }
    }
}