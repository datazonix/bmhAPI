using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace bmhAPI.Services
{
    public class HostelService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;

        public HostelService(IConfiguration configuration, IHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public List<Dictionary<string, object>> GetHostelList(string hostelCode, string cityName)
        {
            string connectionString = _env.IsDevelopment()
                ? _configuration.GetConnectionString("LocalConnection")
                : _configuration.GetConnectionString("RemoteConnection");

            var result = new List<Dictionary<string, object>>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_hostel_list", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@hostel_code", string.IsNullOrEmpty(hostelCode) ? DBNull.Value : hostelCode);
                cmd.Parameters.AddWithValue("@city_name", string.IsNullOrEmpty(cityName) ? DBNull.Value : cityName);

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
