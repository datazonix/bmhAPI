using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;

namespace bmhAPI.Services
{
    public class DatabaseService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public DatabaseService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public List<Dictionary<string, object>> ExecuteStoredProcedure(string procedureName, JsonElement parameters)
        {
            // Choose connection string based on environment
            var connectionString = _env.IsDevelopment()
                ? _configuration.GetConnectionString("LocalConnection")
                : _configuration.GetConnectionString("RemoteConnection");

            var result = new List<Dictionary<string, object>>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters from incoming JSON
                    foreach (var prop in parameters.EnumerateObject())
                    {
                        cmd.Parameters.AddWithValue("@" + prop.Name,
                            prop.Value.ValueKind == JsonValueKind.Null
                                ? DBNull.Value
                                : prop.Value.ToString());
                    }

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }
    }
}
