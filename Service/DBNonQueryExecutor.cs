using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;

namespace bmhAPI.Services
{
    public class DBNonQueryExecutor
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public DBNonQueryExecutor(IConfiguration configuration, IWebHostEnvironment env)
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

        /// <summary>
        /// Executes a stored procedure that may return an output parameter or a result set
        /// </summary>
        public (List<Dictionary<string, object?>>? tableResult, Dictionary<string, object?>? outputParams) ExecuteProcedure(
            string storedProcedure,
            Dictionary<string, object?>? inputParams = null,
            string[]? outputParamNames = null)
        {
            using var conn = new SqlConnection(GetConnectionString());
            using var cmd = new SqlCommand(storedProcedure, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Input parameters
            if (inputParams != null)
            {
                foreach (var param in inputParams)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            // Output parameters
            if (outputParamNames != null)
            {
                foreach (var outParam in outputParamNames)
                {
                    cmd.Parameters.Add(outParam, SqlDbType.NVarChar, 400).Direction = ParameterDirection.Output;
                }
            }

            conn.Open();

            List<Dictionary<string, object?>>? tableResult = null;

            using var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                tableResult = new List<Dictionary<string, object?>>();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object?>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    tableResult.Add(row);
                }
            }

            // Collect output parameters
            Dictionary<string, object?>? outputValues = null;
            if (outputParamNames != null)
            {
                outputValues = new Dictionary<string, object?>();
                foreach (var outParam in outputParamNames)
                {
                    outputValues[outParam] = cmd.Parameters[outParam].Value;
                }
            }

            return (tableResult, outputValues);
        }
    }
}