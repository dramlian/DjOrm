namespace DjOrmTests;

public class TableEntitiesMakerTests
{
    private List<Table> _tables;

    [SetUp]
    public void Setup()
    {
        _tables = [.. new TableEntitiesMaker(new TypeTranslator(), typeof(CarEntity).Assembly)
            .CreateObjectEntities()];
    }

    [Test]
    public void CreateObjectEntities_ReturnsOneTablePerEntity()
    {
        Assert.That(_tables, Has.Count.EqualTo(2));
    }

    [Test]
    public void CreateObjectEntities_TableNames_UseTypeFullName()
    {
        Assert.That(_tables.Select(t => t.TableName), Is.EquivalentTo(
        [
            typeof(CarEntity).FullName,
            typeof(DriverEntity).FullName
        ]));
    }

    [Test]
    public void CreateObjectEntities_PrimaryKey_IsFirstPropertyAndMarkedAsPk()
    {
        var pk = _tables.Single(t => t.TableType == typeof(CarEntity)).Properties.First();

        Assert.That(pk.FullName, Is.EqualTo("Id"));
        Assert.That(pk.IsPrimaryKey, Is.True);
        Assert.That(pk.TranslatedSQLType, Is.EqualTo("INTEGER"));
        Assert.That(pk.IsNullable, Is.False);
    }

    [Test]
    public void CreateObjectEntities_OrdinaryProperties_AreTranslatedCorrectly()
    {
        var ordinaryProps = _tables.Single(t => t.TableType == typeof(CarEntity))
            .Properties.Where(p => !p.IsPrimaryKey).ToList();

        Assert.That(ordinaryProps, Has.Count.EqualTo(2));
        Assert.That(ordinaryProps.Select(p => p.FullName), Is.EquivalentTo(new[] { "Name", "Make" }));
        Assert.That(ordinaryProps.All(p => p.TranslatedSQLType == "TEXT"), Is.True);
    }

    [Test]
    public void CreateObjectEntities_NullableProperty_IsMarkedNullable()
    {
        var nameProp = _tables.Single(t => t.TableType == typeof(DriverEntity))
            .Properties.Single(p => p.FullName == "Name");

        Assert.That(nameProp.IsNullable, Is.True);
    }

    [Test]
    public void CreateObjectEntities_SecondaryKeyProperties_AreExcludedFromProperties()
    {
        var propertyNames = _tables.Single(t => t.TableType == typeof(CarEntity))
            .Properties.Select(p => p.FullName);

        Assert.That(propertyNames, Does.Not.Contain("Driver"));
        Assert.That(propertyNames, Does.Not.Contain("DriverTwo"));
    }

    [Test]
    public void CreateObjectEntities_TableMissingPrimaryKey_ThrowsException()
    {
        var makerWithTestAssembly = new TableEntitiesMaker(new TypeTranslator(), typeof(PersonNoPrimaryKeyEntity).Assembly);

        Assert.Throws<Exception>(() => makerWithTestAssembly.CreateObjectEntities().ToList());
    }
}