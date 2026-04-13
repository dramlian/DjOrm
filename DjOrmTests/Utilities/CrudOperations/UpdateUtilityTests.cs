namespace DjOrmTests;

public class UpdateUtilityTests
{
    private UpdateUtility<EmployeeEntity> _updateUtility;
    private FakeDatabaseConnector _fakeConnector;

    [SetUp]
    public void Setup()
    {
        _fakeConnector = new FakeDatabaseConnector();
        _updateUtility = new UpdateUtility<EmployeeEntity>(_fakeConnector);
    }

    [Test]
    public async Task UpdateData_GeneratesCorrectCommand()
    {
        var employee = new EmployeeEntity { Id = 1, FirstName = "John", LastName = "Doe", Age = 30 };

        await _updateUtility.UpdateData(employee);

        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("UPDATE EmployeeEntity SET FirstName = 'John',LastName = 'Doe',Age = 30 WHERE Id = 1"));
    }

    [Test]
    public async Task UpdateData_WithNullField_SkipsNullInCommand()
    {
        var employee = new EmployeeEntity { Id = 1, FirstName = "John", LastName = null, Age = 30 };

        await _updateUtility.UpdateData(employee);

        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("UPDATE EmployeeEntity SET FirstName = 'John',Age = 30 WHERE Id = 1"));
    }
}