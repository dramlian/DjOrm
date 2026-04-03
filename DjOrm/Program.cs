using Npgsql;

public class DjOrm
{
    public static void Main()
    {
        ITableEntitiesMaker entitiesMaker = new TableEntitiesMaker(new TypeTranslator());
        var entities = entitiesMaker.CreateObjectEntities();
        var commands = new SqlCreateTablesTranslator(entities).TranslateEntitiesToCreateTables().ToList();
        var junctionCommands = new SqlCreateJunctionTableTranslator(entities).TranslateEntitiesToCreateTables().ToList();
        commands.AddRange(junctionCommands);

        var databaseConnector = new DatabaseConnector();

        databaseConnector.ExecuteCommands(commands);

    }
}