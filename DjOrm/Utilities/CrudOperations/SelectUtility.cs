using System.Collections;

public class SelectUtility<T> : ISelectUtility<T>
{
    private IDatabaseConnector dbConnect;
    public SelectUtility(IDatabaseConnector dbConnect)
    {
        this.dbConnect = dbConnect;
    }

    public async Task<IEnumerable<T>> GetAllData()
    {
        var properties = typeof(T).GetProperties()
                       .Where(p => !p.CustomAttributes
                       .Any(a => a.AttributeType == typeof(SecondaryKeyAttribute)))
                       .ToArray();

        var command = $"SELECT {String.Join(",", properties.Select(x => x.Name))} FROM {typeof(T).FullName};";
        var rows = (await dbConnect.GetDataReaderResults(command, properties.Count())).ToArray();

        var result = new List<T>();

        foreach (var row in rows.Cast<ArrayList>())
        {
            var obj = Activator.CreateInstance<T>() ?? throw new InvalidOperationException($"Could not create instance of {typeof(T).Name}");
            for (int i = 0; i < properties.Length; i++)
            {
                var prop = obj.GetType().GetProperty(properties[i].Name);
                prop?.SetValue(obj, Convert.ChangeType(row[i], properties[i].PropertyType));
            }
            result.Add(obj);
        }

        return result;
    }
}