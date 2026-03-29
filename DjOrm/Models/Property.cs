public class Property
{
    public string FullName { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string RunTimeCodeType { get; set; }
    public string TranslatedSQLType { get; set; }

    public Property(string fullName, bool isPrimaryKey, string runTimeCodeType, string translatedSQLType)
    {
        FullName = fullName;
        IsPrimaryKey = isPrimaryKey;
        RunTimeCodeType = runTimeCodeType;
        TranslatedSQLType = translatedSQLType;
    }

}