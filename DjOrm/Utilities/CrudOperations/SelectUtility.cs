using System.Collections;
using System.Reflection;

public class SelectUtility<T> : Utility, ISelectUtility<T>
{
    public SelectUtility(IDatabaseConnector dbConnect) : base(dbConnect)
    {
    }

    public async Task<IEnumerable<T>> GetAllData()
    {
        var properties = GetPropertyInfos();
        var command = $"SELECT {String.Join(",", properties.Select(x => x.Name))} FROM {typeof(T).FullName};";
        var rows = (await _dbConnect.GetDataReaderResults(command, properties.Count())).ToArray();
        return CastRowsIntoObjects(rows, properties);
    }

    protected PropertyInfo[] GetPropertyInfos()
    {
        return typeof(T).GetProperties()
                       .Where(p => !p.CustomAttributes
                       .Any(a => a.AttributeType == typeof(SecondaryKeyAttribute)))
                       .ToArray();
    }

    protected IEnumerable<T> CastRowsIntoObjects(object[] rows, System.Reflection.PropertyInfo[] properties)
    {
        var result = new List<T>();

        foreach (var row in rows.Cast<ArrayList>())
        {
            var obj = Activator.CreateInstance<T>() ??
            throw new InvalidOperationException($"Could not create instance of {typeof(T).Name}");
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