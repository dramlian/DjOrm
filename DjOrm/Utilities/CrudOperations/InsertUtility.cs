using System.Text;

public class InsertUtility<T> : Utility, IInsertUtility<T>
{
    public InsertUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {
    }

    public async Task<(int, string)> InsertInputs(object input)
    {
        if (input is null) return (-1, string.Empty);

        var id = await InsertObj(input);
        var name = GetObjName(input);
        var relations = new List<(int, string)>();
        var fetchedRelations = await GetNameValueOfProperty(input, new List<Type>() { typeof(SecondaryKeyAttribute) });

        if (fetchedRelations is not null && fetchedRelations.Any())
        {
            foreach (var relation in fetchedRelations)
            {
                if (relation.Item2 is null) continue;
                relations.Add(await InsertInputs(relation.Item2));
            }

            foreach (var relation in relations)
            {
                await InsertJunctionObj(id, name, relation.Item1, relation.Item2);
            }
        }

        return (id, name);
    }

    private async Task InsertJunctionObj(int input1Id, string input1Name, int input2Id, string input2Name)
    {
        string command = $"INSERT INTO {input1Name}{input2Name} ({input1Name}Id, {input2Name}Id) VALUES({input1Id}, {input2Id})";
        await _dbConnect.ExecuteCommand(command);
    }

    private async Task<int> InsertObj(object input)
    {
        var properties = await GetNameValueOfPropertyWithoutAttributes(input);

        properties = properties.Where(x => x.Item2 is not null);

        var strBuilder = new StringBuilder();
        strBuilder.Append($"""INSERT INTO {input.GetType().FullName} ({string.Join(",", properties.Select(x => x.Item1))}) VALUES""");


        strBuilder.Append($"({string.Join(",", properties.Select(x => AppendQuotes(x.Item2!, x.Item2!.GetType())))}),");
        var finalValue = strBuilder.ToString();
        finalValue = finalValue[..^1] + "RETURNING Id;";

        return await _dbConnect.ExecuteCommandReturningId(finalValue);
    }
}