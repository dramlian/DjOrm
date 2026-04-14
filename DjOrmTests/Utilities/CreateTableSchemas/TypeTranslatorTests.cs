namespace DjOrmTests;

public class TypeTranslatorTests
{
    private ITypeTranslator _typeTranslator;

    [SetUp]
    public void Setup()
    {
        _typeTranslator = new TypeTranslator();
    }

    [TestCase("Int32", "INTEGER")]
    [TestCase("Int64", "BIGINT")]
    [TestCase("String", "TEXT")]
    [TestCase("Boolean", "BOOLEAN")]
    [TestCase("Double", "DOUBLE PRECISION")]
    [TestCase("Single", "REAL")]
    [TestCase("Decimal", "NUMERIC")]
    [TestCase("DateTime", "TIMESTAMP")]
    [TestCase("Byte[]", "BYTEA")]
    public void TranslateToSql_ReturnsCorrectSqlType(string runtimeType, string expectedSqlType)
    {
        var result = _typeTranslator.TranslateToSql(runtimeType);
        Assert.That(result, Is.EqualTo(expectedSqlType));
    }

    [Test]
    public void TranslateToSql_UnsupportedType_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _typeTranslator.TranslateToSql("Guid"));
    }
}