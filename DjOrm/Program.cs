using DotNetEnv;
public class DjOrm
{
    public static async Task Main()
    {
        Env.Load();

        var connString = Environment.GetEnvironmentVariable("CONN_STRING") ?? throw new Exception("Missing CONN_STRING");
        var db = new DatabaseConnector(connString);
        ITableEntitiesMaker entitiesMaker = new TableEntitiesMaker(new TypeTranslator());
        var entities = entitiesMaker.CreateObjectEntities();
        var commands = new SqlCreateTablesTranslator(entities).TranslateEntitiesToCreateTables().ToList();
        var junctionCommands = new SqlCreateJunctionTableTranslator(entities).TranslateEntitiesToCreateTables().ToList();
        commands.AddRange(junctionCommands);

        var databaseConnector = new DatabaseConnector(connString);
        await databaseConnector.ExecuteCommands(commands);

        IDbContext<CarEntity> dbContext = new DbContext<CarEntity>(databaseConnector);
        await dbContext.InsertData(new CarEntity("prius", "toyota", new DriverEntity("adam")));

        var objectsFetched = await dbContext.GetData();
    }
}