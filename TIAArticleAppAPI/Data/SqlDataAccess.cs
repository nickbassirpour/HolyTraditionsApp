using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace TIAArticleAppAPI.Services
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("ConnectionString");
        }

        public List<T> LoadData<T, U>(string sqlStatement, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                List<T> rows = connection.Query<T>(sqlStatement, parameters, commandType: CommandType.StoredProcedure).ToList();
                return rows;
            }
        }

        public void SaveData<T>(string sqlStatement, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                connection.Execute(sqlStatement, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
