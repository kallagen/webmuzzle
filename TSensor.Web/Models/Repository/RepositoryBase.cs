using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TSensor.Web.Models.Repository
{
    public class RepositoryBase
    {
        private readonly string connectionString;

        public RepositoryBase(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            using IDbConnection db = new SqlConnection(connectionString);

            return db.Query<T>(sql, param);
        }

        public T QueryFirst<T>(string sql, object param = null)
        {
            using IDbConnection db = new SqlConnection(connectionString);

            return db.QueryFirstOrDefault<T>(sql, param);
        }

        public async Task<T> QueryFirstAsync<T>(string sql, object param = null)
        {
            using IDbConnection db = new SqlConnection(connectionString);

            return await db.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public async Task ExecuteAsync(string sql, object param = null)
        {
            using IDbConnection db = new SqlConnection(connectionString);

            await db.ExecuteAsync(sql, param);
        }
    }
}