using System.Reflection;

public class TableEntitiesMaker
{
    private TypeTranslator _typeTranslator;
    private Assembly _assembly;

    public TableEntitiesMaker(TypeTranslator typeTranslator)
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

            Property primaryKey = TranslateTheProperty(primaryKeyRaw);
            var properties = propertiesRaw.Select(x => TranslateTheProperty(x));

            ret.Add(new Table(table.FullName ?? throw new Exception("Could not determine table name"), [primaryKey, .. properties]));
        }
        return ret;
    }

    private Property TranslateTheProperty(PropertyInfo primaryKeyRaw, bool isPk = false)
    {
        return new Property(primaryKeyRaw.Name, isPk, primaryKeyRaw.PropertyType.Name, _typeTranslator.TranslateToSql(primaryKeyRaw.PropertyType.Name));
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
        return tableClass.GetProperties().Where(x => x?.CustomAttributes.Count() == 0);
    }

}