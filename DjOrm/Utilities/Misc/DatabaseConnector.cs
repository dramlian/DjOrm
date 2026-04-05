using Npgsql;

public class DatabaseConnector : IDatabaseConnector
{
    private string _connString;

    public DatabaseConnector(string connString)
    {
        _connString = connString;
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

    public async Task ExecuteCommands(IEnumerable<string> commands)
    {
        using (var conn = new NpgsqlConnection(_connString))
        {
            conn.Open();

            foreach (var command in commands)
            {
                using (var cmd = new NpgsqlCommand(command, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}