using Dapper;
using Microsoft.Data.SqlClient;
using RecipeBook.ServiceLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeBook.ServiceLibrary.Repositories
{
    public interface IRecipeRepository
    {
        Task<int> InsertAsync(RecipeEntity entity);
    }
    public class RecipeRepository : IRecipeRepository
    {
        public async Task<int> InsertAsync(RecipeEntity entity)
        {
            using (var connection = new SqlConnection("Data Source=host.docker.internal,5050;Initial Catalog=RecipeBook;User Id=sa;Password=P@ssword123;TrustServerCertificate=True"))
            {
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
                return rowsAffected;
            }
        }
    }
}
