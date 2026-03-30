public interface ISqlCreateTablesTranslator
{
    public IEnumerable<string> TranslateEntitiesToCreateTables();
}