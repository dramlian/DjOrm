namespace DjOrmTests;

public class SqlCreateTablesTranslatorTests
{
    [Test]
    public void TranslateEntitiesToCreateTables_MultipleTables_ReturnsOneStatementPerTable()
    {
        var tables = new[]
        {
            new Table("PersonEntity",  [new Property("Id", true,  "INTEGER", "INTEGER", false)], typeof(object)),
            new Table("ArticleEntity", [new Property("Id", true,  "INTEGER", "INTEGER", false)], typeof(object))
        };

        var results = new SqlCreateTablesTranslator(tables)
            .TranslateEntitiesToCreateTables()
            .ToList();

        Assert.That(results, Has.Count.EqualTo(2));
    }

    [Test]
    public void TranslateEntitiesToCreateTables_FullTable_GeneratesCorrectSql()
    {
        var table = new Table("PersonEntity", [
            new Property("Id",        true,  "INTEGER", "INTEGER", false),
            new Property("Name",      false, "TEXT",    "TEXT",    false),
            new Property("Biography", false, "TEXT",    "TEXT",    true),
            new Property("Age",       false, "INTEGER", "INTEGER", false),
            new Property("Balance",   false, "NUMERIC", "NUMERIC", false)
        ], typeof(object));

        var result = new SqlCreateTablesTranslator([table])
            .TranslateEntitiesToCreateTables()
            .Single();

        Assert.That(result, Is.EqualTo(
            "CREATE TABLE IF NOT EXISTS PersonEntity(" +
            "Id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY," +
            "Name TEXT NOT NULL," +
            "Biography TEXT ," +
            "Age INTEGER ," +
            "Balance NUMERIC NOT NULL," +
            "created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);"));
    }

    [Test]
    public void TranslateEntitiesToCreateTables_TableWithNullProperties_SkipsTable()
    {
        var table = new Table("EmptyEntity", null!, typeof(object));

        var results = new SqlCreateTablesTranslator([table])
            .TranslateEntitiesToCreateTables()
            .ToList();

        Assert.That(results, Is.Empty);
    }
}
