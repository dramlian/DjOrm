namespace DjOrmTests;

public class SqlCreateJunctionTableTranslatorTests
{
    [Test]
    public void TranslateEntitiesToCreateTables_EmptyInput_ReturnsEmpty()
    {
        var results = new SqlCreateJunctionTableTranslator([])
            .TranslateEntitiesToCreateTables()
            .ToList();

        Assert.That(results, Is.Empty);
    }

    [Test]
    public void TranslateEntitiesToCreateTables_TablesWithNoSecondaryKeys_ReturnsEmpty()
    {
        var tables = new[]
        {
            new Table("PersonEntity",  [], typeof(PersonEntity)),
            new Table("TagEntity",     [], typeof(TagEntity))
        };

        var results = new SqlCreateJunctionTableTranslator(tables)
            .TranslateEntitiesToCreateTables()
            .ToList();

        Assert.That(results, Is.Empty);
    }

    [Test]
    public void TranslateEntitiesToCreateTables_SingleRelationship_GeneratesCorrectSql()
    {
        var tables = new[]
        {
            new Table("ArticleEntity", [], typeof(ArticleEntity)),
            new Table("TagEntity",     [], typeof(TagEntity))
        };

        var results = new SqlCreateJunctionTableTranslator(tables)
            .TranslateEntitiesToCreateTables();

        var result = results.Single();

        var articleFullName = typeof(ArticleEntity).FullName;
        var tagFullName = typeof(TagEntity).FullName;

        Assert.That(result, Is.EqualTo(
            $"""
            CREATE TABLE IF NOT EXISTS {articleFullName}{tagFullName}(
            {articleFullName}Id INTEGER NOT NULL,
            {tagFullName}Id INTEGER NOT NULL,
            PRIMARY KEY ({articleFullName}Id, {tagFullName}Id),
            CONSTRAINT fk_{articleFullName} FOREIGN KEY ({articleFullName}Id) REFERENCES {articleFullName}(Id) ON DELETE CASCADE,
            CONSTRAINT fk_{tagFullName} FOREIGN KEY ({tagFullName}Id) REFERENCES {tagFullName}(Id) ON DELETE CASCADE
            );
        """));
    }

    [Test]
    public void TranslateEntitiesToCreateTables_TableMissingPrimaryKey_ThrowsException()
    {
        var tables = new[]
        {
            new Table("PersonNoPrimaryKeyEntity", [], typeof(PersonNoPrimaryKeyEntity))
        };

        Assert.Throws<Exception>(() =>
            new SqlCreateJunctionTableTranslator(tables)
                .TranslateEntitiesToCreateTables()
                .ToList());
    }
}