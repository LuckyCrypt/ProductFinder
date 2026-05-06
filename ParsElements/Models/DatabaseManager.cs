using Npgsql;

namespace ParsElements.Models
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string host, string user, string password, string dbname)
        {
            _connectionString = $"Host={host};Username={user};Password={password};Database={dbname};Pooling=true;";
        }

        public async Task LogToDb(string message, int userId = 1)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand("INSERT INTO logs (Date, Users, Messages) VALUES (@d, @u, @m)", conn);
                cmd.Parameters.AddWithValue("d", DateTime.Now);
                cmd.Parameters.AddWithValue("u", userId);
                cmd.Parameters.AddWithValue("m", message);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Failed to write log to DB");
            }
        }

        public async Task SaveItems(List<ApartmentItem> items)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            foreach (var item in items)
            {
                // Проверка на дубликат
                await using var checkCmd = new NpgsqlCommand("SELECT 1 FROM apartmentsAvito WHERE apartmentsID = @id", conn);
                checkCmd.Parameters.AddWithValue("id", item.ApartmentsId);
                var exists = await checkCmd.ExecuteScalarAsync();

                if (exists == null)
                {
                    await using var insCmd = new NpgsqlCommand(
                        "INSERT INTO apartmentsAvito (DateScan, name, url, price, requirement, active, apartmentsID) VALUES (@ds, @n, @u, @p, @r, @a, @id)", conn);
                    insCmd.Parameters.AddWithValue("ds", DateTime.Now);
                    insCmd.Parameters.AddWithValue("n", item.Name ?? "");
                    insCmd.Parameters.AddWithValue("u", item.Url ?? "");
                    insCmd.Parameters.AddWithValue("p", item.Price ?? "");
                    insCmd.Parameters.AddWithValue("r", item.Requirement ?? "");
                    insCmd.Parameters.AddWithValue("a", true);
                    insCmd.Parameters.AddWithValue("id", item.ApartmentsId);
                    await insCmd.ExecuteNonQueryAsync();
                }
            }
            await LogToDb($"Saved {items.Count} items to database.");
        }

        public async Task LogHistory(int userId = 1)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("INSERT INTO history (Date, Users) VALUES (@d, @u)", conn);
            cmd.Parameters.AddWithValue("d", DateTime.Now);
            cmd.Parameters.AddWithValue("u", userId);
            await cmd.ExecuteNonQueryAsync();
        }
    }

}
