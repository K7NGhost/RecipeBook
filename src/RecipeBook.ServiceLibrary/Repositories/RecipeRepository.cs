using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RecipeBook.ServiceLibrary.Data;
using RecipeBook.ServiceLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Text;

namespace RecipeBook.ServiceLibrary.Repositories
{
    // Ensure all CRUD operation are properly created
    public interface IRecipeRepository
    {
        Task<int> InsertAsync(RecipeEntity entity);
    }

    // Actual manager that does CRUD operations into the database
    public class RecipeRepository : IRecipeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<RecipeRepository> _logger;

        public RecipeRepository(IDbConnectionFactory connectionFactory, ILogger<RecipeRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentException(nameof(connectionFactory));
            _logger = logger;
        }

        public async Task<int> InsertAsync(RecipeEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            using var connection = _connectionFactory.CreateConnection();

            if (connection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();
                var rowsAffected = await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Recipes]
                           ([Id]
                           ,[Title]
                           ,[Description]
                           ,[Logo]
                           ,[CreatedDate])
                     VALUES
                           (@Id
                           ,@Title
                           ,@Description
                           ,@Logo
                           ,@CreatedDate)",
                   new
                   {
                       entity.Id,
                       entity.Title,
                       entity.Description,
                       entity.Logo,
                       entity.CreatedDate
                   });
                _logger?.LogDebug($"Inserted recipe {entity.Id} ({rowsAffected} rows affected).");
                return rowsAffected;
            }
            throw new InvalidOperationException("The configured IDbConnectionFactory must return a SqlConnection");
        }
    }
}
