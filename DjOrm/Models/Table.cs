public class Table
{
    public string TableName { get; set; }
    public IEnumerable<Property> Properties { get; set; }

    public Table(string tableName, IEnumerable<Property> properties)
    {
        TableName = tableName;
        Properties = properties;
    }
}