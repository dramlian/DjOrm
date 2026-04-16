using System.Collections;
using DjOrmTests;

public class SelectByUtilityTests
{
    private ISelectByUtility _selectByUtility;
    private FakeDatabaseConnector _fakeConnector;

    [SetUp]
    public void Setup()
    {
        _fakeConnector = new FakeDatabaseConnector();
        _selectByUtility = new SelectByUtility(_fakeConnector);
    }

    [Test]
    public async Task GetByExpression_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression<EmployeeEntity>(x => (x.FirstName == "Adam" || x.FirstName == "Peter") && x.Age > 15 && x.LastName != "Thomas");
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE ((((FirstName = 'Adam') OR (FirstName = 'Peter')) AND (Age > 15)) AND (LastName <> 'Thomas'));"));
    }

    [Test]
    public async Task GetByExpression_SingleCondition_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression<EmployeeEntity>(x => x.Age == 30);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age = 30);"));
    }

    [Test]
    public async Task GetByExpression_LessThan_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression<EmployeeEntity>(x => x.Age < 18);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age < 18);"));
    }

    [Test]
    public async Task GetByExpression_LessThanOrEqual_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression<EmployeeEntity>(x => x.Age <= 18);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age <= 18);"));
    }

    [Test]
    public async Task GetByExpression_GreaterThanOrEqual_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression<EmployeeEntity>(x => x.Age >= 65);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age >= 65);"));
    }

    [Test]
    public async Task GetByExpression_CapturedVariable_GeneratesCorrectCommand()
    {
        int minAge = 21;
        await _selectByUtility.GetByExpression<EmployeeEntity>(x => x.Age > minAge);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age > 21);"));
    }

    [Test]
    public async Task GetByExpression_NoExpression_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression<ArticleEntity>(null, true);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM ArticleEntity ;"));
    }

    [Test]
    public async Task GetByExpression_Recursive_IssuesAllThreeCommands()
    {
        _fakeConnector.EnqueueResult([new ArrayList { 1, "Test Article" }]);
        _fakeConnector.EnqueueResult([new ArrayList { 1, 2 }]);
        _fakeConnector.EnqueueResult([new ArrayList { 2, "Tech" }]);

        await _selectByUtility.GetByExpression<ArticleEntity>(null, recursive: true);

        Assert.Multiple(() =>
        {
            Assert.That(_fakeConnector.AllCommands, Has.Count.EqualTo(3));
            Assert.That(_fakeConnector.AllCommands[0], Is.EqualTo("SELECT * FROM ArticleEntity ;"));
            Assert.That(_fakeConnector.AllCommands[1], Is.EqualTo("SELECT * FROM ArticleEntityTagEntity WHERE ArticleEntityId = 1;"));
            Assert.That(_fakeConnector.AllCommands[2], Is.EqualTo("SELECT * FROM TagEntity WHERE (Id = 2);"));
        });
    }
}