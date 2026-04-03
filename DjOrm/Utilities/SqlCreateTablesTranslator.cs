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
        foreach (var table in _tableData.OrderBy(x => x.Properties.Count(p => p is SecondaryProperty)))
        {
            StringBuilder parametersBuilder = new();
            parametersBuilder.Append($"CREATE TABLE IF NOT EXISTS {table.TableName}(");

            if (_tableData?.FirstOrDefault()?.Properties is null) continue;

            foreach (var property in table!.Properties)
            {
                if (property.TranslatedSQLType.Equals("INTEGER"))
                {
                    parametersBuilder.Append($"{property.FullName} INTEGER {(property.IsPrimaryKey ? "GENERATED ALWAYS AS IDENTITY PRIMARY KEY" : string.Empty)},");
                }
                else
                {
                    parametersBuilder.Append($"{property.FullName} {property.TranslatedSQLType} {(property.IsNullable ? string.Empty : "NOT NULL")},");
                }
            }

            var secondaryForeignFields = table.Properties.Where(x => x.GetType() == typeof(SecondaryProperty)).Select(x => (SecondaryProperty)x).ToList();
            bool anyForeignFields = secondaryForeignFields.Any();
            parametersBuilder.Append($"created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP {(anyForeignFields ? "," : ");")}");

            if (anyForeignFields)
            {
                for (int i = 0; i < secondaryForeignFields.Count(); i++)
                {
                    parametersBuilder.Append($"CONSTRAINT fk_{secondaryForeignFields[i].TargetTableName} FOREIGN KEY ({secondaryForeignFields[i].FullName}) REFERENCES " +
                    $"{secondaryForeignFields[i].TargetTableName}({secondaryForeignFields[i].TargetTablePrimaryKeyName}) ON DELETE CASCADE");

                    if (i != secondaryForeignFields.Count() - 1)
                    {
                        parametersBuilder.Append(",");
                    }
                }
                parametersBuilder.Append(");");
            }
            yield return parametersBuilder.ToString();
        }
    }

}