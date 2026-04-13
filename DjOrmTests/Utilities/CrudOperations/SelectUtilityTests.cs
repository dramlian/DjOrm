namespace DjOrmTests;

public class SelectUtilityTests
{
    private SelectUtility<PersonEntity> _selectUtility;
    private FakeDatabaseConnector _fakeConnector;

    [SetUp]
    public void Setup()
    {
        _fakeConnector = new FakeDatabaseConnector();
        _selectUtility = new SelectUtility<PersonEntity>(_fakeConnector);
    }

    [Test]
    public async Task GetAllData_GeneratesCorrectCommand()
    {
        await _selectUtility.GetAllData();
        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("SELECT Id,Name FROM PersonEntity;"));
    }

    //TODO ADD functionality to select also related object, seocndary key object
}