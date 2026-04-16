using System.Text;

public class UpdateUtility<T> : Utility, IUpdateUtility<T>
{
    public UpdateUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {
    }

    public async Task UpdateData(T input)
    {
        if (input is null) return;
        string name = GetObjName(input);
        var pk = GetNameValueOfProperty(input, new List<Type>() { typeof(PrimaryKeyAttribute) })?.First();
        if (pk is null || pk.Value.Item1 is null || pk.Value.Item2 is null) return;
        var attributes = GetNameValueOfPropertyWithoutAttributes(input);

        var command = $"UPDATE {name} SET ";
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(command);

        foreach (var attribute in attributes)
        {
            if (attribute.Item2 is null) continue;
            stringBuilder.Append($"{attribute.Item1} = {AppendQuotes(attribute.Item2, attribute.Item2.GetType())},");
        }

        command = stringBuilder.ToString();
        string result = command.Substring(0, command.Length - 1) + $" WHERE {pk.Value.Item1} = {pk.Value.Item2}";
        await _dbConnect.ExecuteCommand(result);
    }
}