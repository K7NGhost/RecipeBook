using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using RecipeBook.ServiceLibrary.Data;
using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
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

        [Fact]
        public void Constructor_NullConnectionFactory_ThrowsArgumentException()
        {
            var logger = NullLogger<RecipeRepository>.Instance;
            Assert.Throws<ArgumentException>(() => new RecipeRepository(null!, logger));
        }

        [Fact]
        public async Task InsertAsync_NullEntity_ThrowsArgumentNullException()
        {
            var connectionFactory = new FakeConnectionFactory();
            var logger = NullLogger<RecipeRepository>.Instance;
            var repo = new RecipeRepository(connectionFactory, logger);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.InsertAsync(null!));
        }

        [Fact]
        public async Task InsertAsync_NonSqlConnection_ThrowsInvalidOperationException()
        {
            var connectionFactory = new FakeConnectionFactory(); // returns a non-Sql connection
            var logger = NullLogger<RecipeRepository>.Instance;
            var repo = new RecipeRepository(connectionFactory, logger);

            var entity = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Desc",
                CreatedDate = DateTimeOffset.UtcNow
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.InsertAsync(entity));
        }

        [Fact]
        public async Task GetByIdAsync_NonSqlConnection_ReturnsNull()
        {
            var connectionFactory = new FakeConnectionFactory();
            var logger = NullLogger<RecipeRepository>.Instance;
            var repo = new RecipeRepository(connectionFactory, logger);

            var result = await repo.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_NonSqlConnection_ReturnsEmpty()
        {
            var connectionFactory = new FakeConnectionFactory();
            var logger = NullLogger<RecipeRepository>.Instance;
            var repo = new RecipeRepository(connectionFactory, logger);

            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_NullEntity_ThrowsArgumentNullException()
        {
            var connectionFactory = new FakeConnectionFactory();
            var logger = NullLogger<RecipeRepository>.Instance;
            var repo = new RecipeRepository(connectionFactory, logger);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.UpdateAsync(null!));
        }

        [Fact]
        public async Task UpdateAsync_NonSqlConnection_ReturnsZero()
        {
            var connectionFactory = new FakeConnectionFactory();
            var logger = NullLogger<RecipeRepository>.Instance;
            var repo = new RecipeRepository(connectionFactory, logger);

            var entity = new RecipeEntity
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Desc",
                CreatedDate = DateTimeOffset.UtcNow
            };

            var rows = await repo.UpdateAsync(entity);

            Assert.Equal(0, rows);
        }

        [Fact]
        public async Task DeleteAsync_NonSqlConnection_ReturnsZero()
        {
            var connectionFactory = new FakeConnectionFactory();
            var logger = NullLogger<RecipeRepository>.Instance;
            var repo = new RecipeRepository(connectionFactory, logger);

            var rows = await repo.DeleteAsync(Guid.NewGuid());

            Assert.Equal(0, rows);
        }

        // Simple test helper implementations to avoid requiring a real SqlConnection in unit tests.
        // These intentionally do NOT inherit from SqlConnection, so the repository falls into the non-Sql branches.
        private class FakeConnectionFactory : IDbConnectionFactory
        {
            public IDbConnection CreateConnection() => new FakeDbConnection();
        }

        private class FakeDbConnection : IDbConnection
        {
            public string ConnectionString { get; set; } = string.Empty;
            public int ConnectionTimeout => 30;
            public string Database => "FakeDb";
            public ConnectionState State => ConnectionState.Closed;

            public IDbTransaction BeginTransaction() => throw new NotImplementedException();
            public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotImplementedException();
            public void ChangeDatabase(string databaseName) => throw new NotImplementedException();
            public void Close() { }
            public IDbCommand CreateCommand() => throw new NotImplementedException();
            public void Open() { }
            public void Dispose() { }
        }
    }
}
