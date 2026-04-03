using System.Runtime.CompilerServices;

public class SqlCreateJunctionTableTranslator : ISqlCreateTablesTranslator
{
    private IEnumerable<Table> _tableData;

    public SqlCreateJunctionTableTranslator(IEnumerable<Table> tableData)
    {
        _tableData = tableData;
    }
    public IEnumerable<string> TranslateEntitiesToCreateTables()
    {
        return GetJunctionTableCommands(GetPrimaryKeysOfTables(), GetTableRelationShips());
    }

    private Dictionary<Type, string> GetPrimaryKeysOfTables()
    {
        var ret = new Dictionary<Type, string>();
        foreach (var table in _tableData)
        {
            ret.Add(table.TableType, table.TableType.GetProperties().Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(PrimaryKeyAttribute)))
            ?.FirstOrDefault()?.Name ?? throw new Exception("Cant get to the Primary Key"));
        }
        return ret.Where(x => x.Value.Count() > 0).ToDictionary();
    }

    private Dictionary<Type, HashSet<Type>> GetTableRelationShips()
    {
        var ret = new Dictionary<Type, HashSet<Type>>();

        foreach (var table in _tableData)
        {
            ret.Add(table.TableType, table.TableType.GetProperties().Where(x => x.CustomAttributes.Any(y => y.AttributeType
            == typeof(SecondaryKeyAttribute))).Select(z => z.PropertyType).ToHashSet());
        }
        return ret;
    }

    private IEnumerable<string> GetJunctionTableCommands(Dictionary<Type, string> primaryKeys, Dictionary<Type, HashSet<Type>> relations)
    {
        foreach (var relation in relations)
        {
            foreach (var innerRelation in relations[relation.Key])
            {
                var type1 = relation.Key;
                var type2 = innerRelation;
                yield return $"""
                CREATE TABLE IF NOT EXISTS {type1.FullName}{type2.FullName}(
                {type1.FullName}Id INTEGER NOT NULL,
                {type2.FullName}Id INTEGER NOT NULL,
                PRIMARY KEY ({type1.FullName}Id, {type2.FullName}Id),
                CONSTRAINT fk_{type1.FullName} FOREIGN KEY ({type1.FullName}Id) REFERENCES {type1.FullName}({primaryKeys[type1]}) ON DELETE CASCADE,
                CONSTRAINT fk_{type2.FullName} FOREIGN KEY ({type2.FullName}Id) REFERENCES {type2.FullName}({primaryKeys[type2]}) ON DELETE CASCADE
                );
                """;
            }
        }
    }
}