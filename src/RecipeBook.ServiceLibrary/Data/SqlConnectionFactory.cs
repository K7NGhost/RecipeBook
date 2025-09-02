using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace RecipeBook.ServiceLibrary.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RecipeBook")
                ?? configuration["RecipeBook:ConnectionString"]
                ?? throw new InvalidOperationException("Connection string 'RecipeBook' not found.");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}