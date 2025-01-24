using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace TIAArticleAppAPI.Data
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

        public List<T> LoadDataList<T, U>(string sqlStatement, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                List<T> rows = connection.Query<T>(sqlStatement, parameters, commandType: CommandType.StoredProcedure).ToList();
                return rows;
            }
        }
        public T LoadDataObject<T, U>(string sqlStatement, U parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                T row = connection.QueryFirst<T>(sqlStatement, parameters, commandType: CommandType.StoredProcedure);
                return row;
            }
        }

        public async Task SaveData<T>(string sqlStatement, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sqlStatement, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
