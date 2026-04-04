using Npgsql;

public class DjOrm
{
    public static async Task Main()
    {
        // ITableEntitiesMaker entitiesMaker = new TableEntitiesMaker(new TypeTranslator());
        // var entities = entitiesMaker.CreateObjectEntities();
        // var commands = new SqlCreateTablesTranslator(entities).TranslateEntitiesToCreateTables().ToList();
        // var junctionCommands = new SqlCreateJunctionTableTranslator(entities).TranslateEntitiesToCreateTables().ToList();
        // commands.AddRange(junctionCommands);

        var databaseConnector = new DatabaseConnector();
        //databaseConnector.ExecuteCommands(commands);

        IDbContext<CarEntity> dbContext = new DbContext<CarEntity>(databaseConnector);
        await dbContext.InsertData(new CarEntity("prius", "toyota", new DriverEntity("adam")));
    }
}