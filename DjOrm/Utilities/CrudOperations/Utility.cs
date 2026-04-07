public abstract class Utility
{
    protected IDatabaseConnector _dbConnect;

    public Utility(IDatabaseConnector dbConnect)
    {
        _dbConnect = dbConnect;
    }

    protected async Task<IEnumerable<(string, object)>> GetNameValueOfProperty(object input, IEnumerable<Type> propertyAttributes)
    {
        return input?.GetType()
               .GetProperties()
               .Where(p => p.CustomAttributes
               .Any(a => propertyAttributes.Contains(a.AttributeType)))
               .Select(x => (x.Name, GetValueOfProperty(input, x.Name)))
            ?? throw new Exception($"Could not get the properties of {input}");
    }

    protected object GetValueOfProperty(object input, string name)
    {
        return input?.GetType()?.GetProperty(name)?.GetValue(input) ?? throw new Exception("");
    }

    protected object AppendQuotes(object obj, Type propertyType)
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