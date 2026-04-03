public class Table
{
    public string TableName { get; set; }
    public IEnumerable<Property> Properties { get; set; }
    public Type TableType { get; set; }

    public Table(string tableName, IEnumerable<Property> properties, Type tableType)
    {
        TableName = tableName;
        Properties = properties;
        TableType = tableType;
    }
}