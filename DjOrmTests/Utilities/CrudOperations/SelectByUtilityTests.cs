using DjOrmTests;

public class SelectByUtilityTests
{
    private ISelectByUtility<EmployeeEntity> _selectByUtility;
    private FakeDatabaseConnector _fakeConnector;

    [SetUp]
    public void Setup()
    {
        _fakeConnector = new FakeDatabaseConnector();
        _selectByUtility = new SelectByUtility<EmployeeEntity>(_fakeConnector);
    }

    [Test]
    public async Task GetByExpression_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression(x => (x.FirstName == "Adam" || x.FirstName == "Peter") && x.Age > 15 && x.LastName != "Thomas");
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE ((((FirstName = 'Adam') OR (FirstName = 'Peter')) AND (Age > 15)) AND (LastName <> 'Thomas'));"));
    }

    [Test]
    public async Task GetByExpression_SingleCondition_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression(x => x.Age == 30);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age = 30);"));
    }

    [Test]
    public async Task GetByExpression_LessThan_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression(x => x.Age < 18);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age < 18);"));
    }

    [Test]
    public async Task GetByExpression_LessThanOrEqual_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression(x => x.Age <= 18);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age <= 18);"));
    }

    [Test]
    public async Task GetByExpression_GreaterThanOrEqual_GeneratesCorrectCommand()
    {
        await _selectByUtility.GetByExpression(x => x.Age >= 65);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age >= 65);"));
    }

    [Test]
    public async Task GetByExpression_CapturedVariable_GeneratesCorrectCommand()
    {
        int minAge = 21;
        await _selectByUtility.GetByExpression(x => x.Age > minAge);
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT * FROM EmployeeEntity WHERE (Age > 21);"));
    }
}