using System.Collections;
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

    public async Task<IEnumerable<object>> GetDataReaderResults(string command, int propertiesCount)
    {
        using (var conn = new NpgsqlConnection(_connString))
        {
            conn.Open();
            using var cmd = new NpgsqlCommand(command, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            List<object> retObj = new List<object>();

            while (await reader.ReadAsync())
            {
                ArrayList row = new ArrayList();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader[i];
                    row.Add(value);
                }

                retObj.Add(row);
            }

            return retObj;
        }
    }
}