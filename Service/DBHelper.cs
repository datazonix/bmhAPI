using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace bmhAPI.Services
{
    public static class DBHelper
    {
        public static SqlCommand GetCommand(Dictionary<string, object> parameters, string procedureName, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            return cmd;
        }
    }
}
