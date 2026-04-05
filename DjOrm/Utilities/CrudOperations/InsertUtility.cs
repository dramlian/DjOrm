using System.Text;

public class InsertUtility<T> //Todo add interface
{
    private IDatabaseConnector dbConnect;

    public InsertUtility(IDatabaseConnector dbConnect)
    {
        this.dbConnect = dbConnect;
    }

    public async Task<(int, string)> InsertInputs(object input)
    {
        if (input is null) return (-1, string.Empty);

        var id = await InsertObj(input);

        var name = GetObjName(input);

        var relations = new List<(int, string)>();

        foreach (var relation in await GetAllRelationObjs(input))
        {
            relations.Add(await InsertInputs(relation));
        }

        foreach (var relation in relations)
        {
            await InsertJunctionObj(id, name, relation.Item1, relation.Item2);
        }

        return (id, name);
    }

    private string GetObjName(object input)
    {
        return input.GetType().FullName
        ?? throw new Exception("Could not get the name of the entity");
    }

    private async Task InsertJunctionObj(int input1Id, string input1Name, int input2Id, string input2Name)
    {
        string command = $"INSERT INTO {input1Name}{input2Name} ({input1Name}Id, {input2Name}Id) VALUES({input1Id}, {input2Id})";
        await dbConnect.ExecuteCommand(command);
    }

    private async Task<IEnumerable<object?>> GetAllRelationObjs(object input)
    {
        var properties = input?.GetType()
               .GetProperties()
               .Where(p => p.CustomAttributes
               .Any(a => a.AttributeType == typeof(SecondaryKeyAttribute)))
               .Select(x => GetValueOfProperty(input, x.Name));

        return properties?.Where(x => x is not null) ?? Enumerable.Empty<object>();
    }

    private async Task<int> InsertObj(object input)
    {
        var properties = input!.GetType()
                       .GetProperties()
                       .Where(p => !p.CustomAttributes
                       .Any(a => a.AttributeType == typeof(PrimaryKeyAttribute)
                        || a.AttributeType == typeof(SecondaryKeyAttribute)))
                       .Select(x => (x.Name, x.PropertyType, GetValueOfProperty(input, x.Name)));

        properties = properties.Where(x => x.Item3 is not null);

        var strBuilder = new StringBuilder();
        strBuilder.Append($"""INSERT INTO {input.GetType().FullName} ({string.Join(",", properties.Select(x => x.Name))}) VALUES""");


        strBuilder.Append($"({string.Join(",", properties.Select(x => AppendQuotes(x.Item3!, x.PropertyType)))}),");
        var finalValue = strBuilder.ToString();
        finalValue = finalValue[..^1] + "RETURNING Id;";

        return await dbConnect.ExecuteCommandReturningId(finalValue);
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

    private object? GetValueOfProperty(object input, string name)
    {
        return input?.GetType()?.GetProperty(name)?.GetValue(input);
    }
}