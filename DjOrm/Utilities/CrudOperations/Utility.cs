public abstract class Utility
{
    protected IDatabaseConnector _dbConnect;

    public Utility(IDatabaseConnector dbConnect)
    {
        _dbConnect = dbConnect;
    }

    protected IEnumerable<(string, object?)>? GetNameValueOfProperty(object input, IEnumerable<Type> propertyAttributes)
    {
        var properties = input?.GetType()
               .GetProperties()?
               .Where(p => p.CustomAttributes
               .Any(a => propertyAttributes.Contains(a.AttributeType)))?
               .Select(x => (x.Name, GetValueOfProperty(input, x.Name))) ?? Enumerable.Empty<(string, object)>();

        return properties?.Where(x => x.Item2 is not null);
    }

    protected IEnumerable<(string, object?)> GetNameValueOfPropertyWithoutAttributes(object input)
    {
        return input.GetType()
            .GetProperties()
            .Where(p => !p.CustomAttributes.Any(a =>
                a.AttributeType == typeof(PrimaryKeyAttribute) ||
                a.AttributeType == typeof(SecondaryKeyAttribute)))
            .Select(x => (x.Name, GetValueOfProperty(input, x.Name)));
    }

    protected object? GetValueOfProperty(object input, string name)
    {
        return input?.GetType()?.GetProperty(name)?.GetValue(input);
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

    protected string GetObjName(object input)
    {
        return input.GetType().FullName
        ?? throw new Exception("Could not get the name of the entity");
    }
}