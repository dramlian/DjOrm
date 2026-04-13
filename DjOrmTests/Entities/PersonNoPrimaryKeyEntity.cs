[TableAttribute]
public class PersonNoPrimaryKeyEntity
{
    public string Name { get; set; } = string.Empty;

    public PersonNoPrimaryKeyEntity() { }

    public PersonNoPrimaryKeyEntity(string name)
    {
        Name = name;
    }
}