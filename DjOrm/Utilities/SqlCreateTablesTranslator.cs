using System.Text;

public class SqlCreateTablesTranslator : ISqlCreateTablesTranslator
{
    private IEnumerable<Table> _tableData;

    public SqlCreateTablesTranslator(IEnumerable<Table> tableData)
    {
        _tableData = tableData;
    }

    public IEnumerable<string> TranslateEntitiesToCreateTables()
    {
        foreach (var table in _tableData)
        {
            StringBuilder parametersBuilder = new();
            parametersBuilder.Append($"CREATE TABLE IF NOT EXISTS {table.TableName}(");

            if (_tableData?.FirstOrDefault()?.Properties is null) continue;

            foreach (var property in _tableData.FirstOrDefault()!.Properties)
            {
                if (property.TranslatedSQLType.Equals("INTEGER"))
                {
                    parametersBuilder.Append($"{property.FullName} INTEGER {(property.IsPrimaryKey ? "PRIMARY KEY AUTOINCREMENT" : string.Empty)},");
                }
                else
                {
                    parametersBuilder.Append($"{property.FullName} {property.TranslatedSQLType} {(property.IsNullable ? string.Empty : "NOT NULL")},");
                }
            }
            parametersBuilder.Append("created_at DATETIME DEFAULT CURRENT_TIMESTAMP );");
            yield return parametersBuilder.ToString();
        }
    }

}