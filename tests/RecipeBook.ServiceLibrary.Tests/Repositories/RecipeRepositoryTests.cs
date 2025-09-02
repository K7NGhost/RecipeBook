using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using RecipeBook.ServiceLibrary.Data;
using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RecipeBook.ServiceLibrary.Tests.Repositories
{
    public class RecipeRepositoryTests
    {
        [Fact]
        public async Task InsertAsync_Success()
        {
            // Provide a test connection string here (or set via environment / secrets)
            var inMemorySettings = new Dictionary<string, string>
            {
                ["ConnectionStrings:RecipeBook"] = "Server=host.docker.internal,5050;Database=RecipeBook;User Id=sa;Password=P@ssword123;TrustServerCertificate=True;"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var connectionFactory = new SqlConnectionFactory(configuration);
            var logger = NullLogger<RecipeRepository>.Instance;

            var recipeRepository = new RecipeRepository(connectionFactory, logger);

            var rowsAffected = await recipeRepository.InsertAsync(new RecipeEntity()
            {
                Id = Guid.NewGuid(),
                Title = "Fried Chicken Unit Test",
                Description = "Fired Chicken Descirption",
                Logo = null,
                CreatedDate = DateTimeOffset.UtcNow
            });

            Assert.Equal(1, rowsAffected);
        }
    }
}
