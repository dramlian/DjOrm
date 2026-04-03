public class TypeTranslator : ITypeTranslator
{
    private static readonly Dictionary<string, string> TypeMap = new()
    {
        { "Int32",    "INTEGER" },
        { "Int64",    "BIGINT" },
        { "String",   "TEXT" },
        { "Boolean",  "BOOLEAN" },
        { "Double",   "DOUBLE PRECISION" },
        { "Single",   "REAL" },
        { "Decimal",  "NUMERIC" },
        { "DateTime", "TIMESTAMP" },
        { "Byte[]",   "BYTEA" }
    };

    public string TranslateToSql(string runTimeCodeType)
    {
        if (TypeMap.TryGetValue(runTimeCodeType, out var sqlType))
            return sqlType;

        throw new NotSupportedException($"Type '{runTimeCodeType}' is not supported.");
    }
}