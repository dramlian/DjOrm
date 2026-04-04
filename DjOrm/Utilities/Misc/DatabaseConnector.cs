using Npgsql;

public class DatabaseConnector : IDatabaseConnector
{
    private string _connString;

    public DatabaseConnector()
    {
        //TODO from env
        _connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=testdb";
    }

    public async Task<int> ExecuteCommandReturningId(string command)
    {
        using (var conn = new NpgsqlConnection(_connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(command, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
    }

    public async Task ExecuteCommand(string command)
    {
        using (var conn = new NpgsqlConnection(_connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand(command, conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}