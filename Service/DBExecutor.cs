using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace bmhAPI.Services
{
    public class DBExecutor
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public DBExecutor(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private string GetConnectionString()
        {
            return _env.IsDevelopment()
                ? _configuration.GetConnectionString("LocalConnection")
                : _configuration.GetConnectionString("RemoteConnection");
        }

        public bool ExecuteNonQuery(SqlCommand cmd)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                cmd.Connection = conn;
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<Dictionary<string, object>> ExecuteReader(SqlCommand cmd)
        {
            var result = new List<Dictionary<string, object>>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                cmd.Connection = conn;
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

            return result;
        }
    }
}
