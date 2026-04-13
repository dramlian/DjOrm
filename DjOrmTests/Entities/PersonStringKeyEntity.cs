[TableAttribute]
public class PersonStringKeyEntity
{
    [PrimaryKeyAttribute]
    public string Name { get; set; } = string.Empty;

    public PersonStringKeyEntity() { }

    public PersonStringKeyEntity(string name)
    {
        Name = name;
    }
}