using Npgsql;

public class DatabaseConnector : IDatabaseConnector
{
    private string _connString;

    public DatabaseConnector()
    {
        _connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=testdb";
    }

    public void ExecuteCommands(IEnumerable<string> commands)
    {
        using (var conn = new NpgsqlConnection(_connString))
        {
            conn.Open();
            foreach (var command in commands)
            {
                using (var cmd = new NpgsqlCommand(command, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}