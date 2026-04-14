namespace DjOrmTests;

public class DeleteUtilityTests
{
    private IDeleteUtility<PersonEntity> _deleteUtility;
    private FakeDatabaseConnector _fakeConnector;

    [SetUp]
    public void Setup()
    {
        _fakeConnector = new FakeDatabaseConnector();
        _deleteUtility = new DeleteUtility<PersonEntity>(_fakeConnector);
    }

    [Test]
    public async Task DeleteData_GeneratesCorrectCommand()
    {
        var person = new PersonEntity { Id = 1 };

        await _deleteUtility.DeleteData(person);

        Assert.That(_fakeConnector.LastCommand, Is.EqualTo("DELETE FROM PersonEntity WHERE Id = 1;"));
    }

    [Test]
    public async Task DeleteData_WithStringPrimaryKey_GeneratesCorrectCommand()
    {
        var connector = new FakeDatabaseConnector();
        var deleteUtility = new DeleteUtility<PersonStringKeyEntity>(connector);
        var person = new PersonStringKeyEntity { Name = "John" };

        await deleteUtility.DeleteData(person);

        Assert.That(connector.LastCommand, Is.EqualTo("DELETE FROM PersonStringKeyEntity WHERE Name = 'John';"));
    }

    [Test]
    public void DeleteData_WithNoPrimaryKey_ThrowsInvalidOperationException()
    {
        var connector = new FakeDatabaseConnector();
        var deleteUtility = new DeleteUtility<PersonNoPrimaryKeyEntity>(connector);
        var person = new PersonNoPrimaryKeyEntity { Name = "John" };

        Assert.ThrowsAsync<InvalidOperationException>(() => deleteUtility.DeleteData(person));
    }
}