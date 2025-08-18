using System.Data;
using Microsoft.Data.SqlClient;

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
        string? connectionString = _env.IsDevelopment()
            ? _configuration.GetConnectionString("LocalConnection")
            : _configuration.GetConnectionString("RemoteConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database connection string is not configured.");

        return connectionString;
    }

    // New ExecuteReader method
    public List<Dictionary<string, object?>> ExecuteReader(string storedProcedure, string[] paramNames, object?[] paramValues)
    {
        if (paramNames.Length != paramValues.Length)
            throw new ArgumentException("Parameter names and values count mismatch.");

        var result = new List<Dictionary<string, object?>>();

        using var conn = new SqlConnection(GetConnectionString());
        using var cmd = new SqlCommand(storedProcedure, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Add parameters
        for (int i = 0; i < paramNames.Length; i++)
        {
            cmd.Parameters.Add(paramNames[i], SqlDbType.VarChar).Value =
                paramValues[i] ?? DBNull.Value;
        }

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            result.Add(row);
        }

        return result;
    }
}
