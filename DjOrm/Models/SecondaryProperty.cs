public class SecondaryProperty : Property
{
    public string TargetTablePrimaryKeyName { get; set; }
    public string TargetTableName { get; set; }

    public SecondaryProperty(
        string fullName,
        bool isNullable,
        string targetTableName,
        string targetTablePrimaryKeyName
    ) : base(fullName, false, string.Empty, "INTEGER", isNullable)
    {
        TargetTableName = targetTableName;
        TargetTablePrimaryKeyName = targetTablePrimaryKeyName;


        this.IsPrimaryKey = false;
    }
}