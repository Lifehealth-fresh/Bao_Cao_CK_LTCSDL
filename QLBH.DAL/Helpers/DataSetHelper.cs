using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace QLBH.DAL.Helpers
{
    public class DataSetHelper
    {
        private readonly string _connectionString;

        public DataSetHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public DataTable GetDataTable(string sql, params SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var adapter = new SqlDataAdapter(sql, conn);
            if (parameters != null && parameters.Length > 0)
                adapter.SelectCommand.Parameters.AddRange(parameters);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public DataSet GetDataSet(string sql, params SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var adapter = new SqlDataAdapter(sql, conn);
            if (parameters != null && parameters.Length > 0)
                adapter.SelectCommand.Parameters.AddRange(parameters);
            var ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
    }
}