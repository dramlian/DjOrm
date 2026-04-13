[TableAttribute]
public class ArticleEntity
{
    [PrimaryKeyAttribute]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    [SecondaryKeyAttribute]
    public TagEntity? Tag { get; set; }

    public ArticleEntity() { }

    public ArticleEntity(string title, TagEntity? tag = null)
    {
        Title = title;
        Tag = tag;
    }
}
