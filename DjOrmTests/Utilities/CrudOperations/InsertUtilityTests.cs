namespace DjOrmTests;

public class InsertUtilityTests
{
    private IInsertUtility<PersonEntity> _insertUtility;
    private FakeDatabaseConnector _fakeConnector;

    [SetUp]
    public void Setup()
    {
        _fakeConnector = new FakeDatabaseConnector();
        _insertUtility = new InsertUtility<PersonEntity>(_fakeConnector);
    }

    [Test]
    public async Task InsertInputs_SimpleObject_GeneratesCorrectInsertCommand()
    {
        var person = new PersonEntity { Name = "John" };

        await _insertUtility.InsertInputs(person);

        Assert.That(_fakeConnector.AllCommands, Has.Count.EqualTo(1));
        Assert.That(_fakeConnector.AllCommands[0], Is.EqualTo("INSERT INTO PersonEntity (Name) VALUES('John')RETURNING Id;"));
    }

    [Test]
    public async Task InsertInputs_WithRelation_InsertsRelatedEntityFirst()
    {
        var article = new ArticleEntity("Clean Code", new TagEntity("tech"));
        var utility = new InsertUtility<ArticleEntity>(_fakeConnector);

        await utility.InsertInputs(article);

        Assert.That(_fakeConnector.AllCommands, Has.Count.EqualTo(3));
        Assert.That(_fakeConnector.AllCommands[0], Is.EqualTo("INSERT INTO ArticleEntity (Title) VALUES('Clean Code')RETURNING Id;"));
        Assert.That(_fakeConnector.AllCommands[2], Is.EqualTo("INSERT INTO ArticleEntityTagEntity (ArticleEntityId, TagEntityId) VALUES(0, 0)"));
        Assert.That(_fakeConnector.AllCommands[1], Is.EqualTo("INSERT INTO TagEntity (Label) VALUES('tech')RETURNING Id;"));
    }
}
