using Microsoft.Data.Sqlite;

public class DjOrm
{
    public static void Main()
    {
        TableEntitiesMaker entitiesMaker = new TableEntitiesMaker(new TypeTranslator());
        var entities = entitiesMaker.CreateObjectEntities();
        var command = new SqlCreateTablesTranslator(entities).TranslateEntitiesToCreateTables();


        using var connection = new SqliteConnection("Data Source=mydb.db");
        connection.Open();
        using var commandsql = new SqliteCommand(command.First(), connection);
        commandsql.ExecuteNonQuery();


        string verifySQL = "SELECT name FROM sqlite_master WHERE type='table';";
        using var verifyCommand = new SqliteCommand(verifySQL, connection);
        using var reader = verifyCommand.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine($"Table found: {reader["name"]}");
        }
    }
}