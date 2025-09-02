using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeBook.ServiceLibrary.Tests.Repositories
{
    public class RecipeRepositoryTests
    {
        [Fact]
        public async Task InsertAsync_Success()
        {
            var recipeRepository = new RecipeRepository();
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
