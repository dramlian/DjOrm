public class TypeTranslator
{
    private static readonly Dictionary<string, string> TypeMap = new()
    {
        { "Int32",    "INTEGER" },
        { "Int64",    "INTEGER" },
        { "String",   "TEXT" },
        { "Boolean",  "INTEGER" },
        { "Double",   "REAL" },
        { "Single",   "REAL" },
        { "Decimal",  "REAL" },
        { "DateTime", "TEXT" },
        { "Byte[]",   "BLOB" }
    };

    public string TranslateToSql(string runTimeCodeType)
    {
        if (TypeMap.TryGetValue(runTimeCodeType, out var sqlType))
            return sqlType;

        throw new NotSupportedException($"Type '{runTimeCodeType}' is not supported.");
    }
}