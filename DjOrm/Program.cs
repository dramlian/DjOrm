using Npgsql;

public class DjOrm
{
    public static void Main()
    {
        ITableEntitiesMaker entitiesMaker = new TableEntitiesMaker(new TypeTranslator());
        var entities = entitiesMaker.CreateObjectEntities();
        var command = new SqlCreateTablesTranslator(entities).TranslateEntitiesToCreateTables().ToList();

        var junctionCommands = new SqlCreateJunctionTableTranslator(entities).TranslateEntitiesToCreateTables().ToList();
        
        // var connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=testdb";
        // using var conn = new NpgsqlConnection(connString);
        // conn.Open();

        // using var cmd = new NpgsqlCommand(command.FirstOrDefault(), conn);
        // cmd.ExecuteNonQuery();
    }
}