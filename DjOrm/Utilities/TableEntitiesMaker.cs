using System.Reflection;

public class TableEntitiesMaker : ITableEntitiesMaker
{
    private ITypeTranslator _typeTranslator;
    private Assembly _assembly;

    public TableEntitiesMaker(ITypeTranslator typeTranslator)
    {
        _typeTranslator = typeTranslator;
        _assembly = Assembly.GetExecutingAssembly();
    }

    public IEnumerable<Table> CreateObjectEntities()
    {
        List<Table> ret = new();
        foreach (var table in ScanAllClasses())
        {
            var primaryKeyRaw = GetPKPropertiesOfTable(table);
            var propertiesRaw = ScanNonPKPropertiesOfTable(table);

            Property primaryKey = TranslateTheProperty(primaryKeyRaw, true);
            var properties = propertiesRaw.Select(x => TranslateTheProperty(x));

            ret.Add(new Table(table.FullName ?? throw new Exception("Could not determine table name"), [primaryKey, .. properties]));
        }
        return ret;
    }

    private Property TranslateTheProperty(PropertyInfo propertyInfo, bool isPk = false)
    {
        return new Property(propertyInfo.Name, isPk, propertyInfo.PropertyType.Name,
        _typeTranslator.TranslateToSql(propertyInfo.PropertyType.Name), IsNullable(propertyInfo));
    }

    private bool IsNullable(PropertyInfo propertyInfo)
    {
        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(propertyInfo);

        return nullabilityInfo.WriteState == NullabilityState.Nullable
                       || nullabilityInfo.ReadState == NullabilityState.Nullable; ;
    }

    public IEnumerable<Type> ScanAllClasses()
    {
        return _assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TableAttribute>() != null);
    }

    public PropertyInfo GetPKPropertiesOfTable(Type tableClass)
    {
        var pkProperties = tableClass.GetProperties().Where(x => x?.CustomAttributes?.FirstOrDefault()?.AttributeType == typeof(PrimaryKeyAttribute));

        if (pkProperties.Count() != 1)
        {
            throw new Exception("Must have one primary key and one key only!");
        }
        return pkProperties.First();
    }

    public IEnumerable<PropertyInfo> ScanNonPKPropertiesOfTable(Type tableClass)
    {
        return tableClass.GetProperties().Where(x =>
            !x.CustomAttributes.Any(a => a.AttributeType == typeof(PrimaryKeyAttribute)));
    }

}