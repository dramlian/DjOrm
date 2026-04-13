[TableAttribute]
public class TagEntity
{
    [PrimaryKeyAttribute]
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;

    public TagEntity() { }

    public TagEntity(string label)
    {
        Label = label;
    }
}
