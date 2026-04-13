[TableAttribute]
public class PersonEntity
{
    [PrimaryKeyAttribute]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public PersonEntity() { }

    public PersonEntity(string name)
    {
        Name = name;
    }
}