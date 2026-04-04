using System.Security.Principal;
using System.Text;

public class InsertUtility<T>
{
    private IDatabaseConnector dbConnect;

    public InsertUtility(IDatabaseConnector dbConnect)
    {
        this.dbConnect = dbConnect;
    }

    public async Task InsertInputs(T input)
    {
        if (input is null) return;

        var id = await InsertObj(input);

        var relations = await GetAllRelationObjs(input);

        foreach (var relation in relations)
        {
            var idRelation = await InsertObj(relation);
        }

        /*   
        1. Take the mechanism modularize it and return ID
        2. Take all secondarykeyattrrbiutes and run recursion
        3. return you get tuple(normal inserts, relationship inserts)

        4. insert normal table
        5. insert  relationships
        */
    }

    private async Task<IEnumerable<object>> GetAllRelationObjs(T input)
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