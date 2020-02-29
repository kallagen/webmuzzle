using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

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
    }
}