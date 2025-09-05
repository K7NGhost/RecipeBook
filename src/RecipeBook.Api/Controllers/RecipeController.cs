using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using System;
using System.Threading.Tasks;

namespace RecipeBook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository ?? throw new ArgumentNullException(nameof(recipeRepository));
        }

        // health check: GET api/recipe/health
        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "healthy" });

        // POST api/recipe
        [HttpPost]
        public async Task<IActionResult> AddNewRecipe([FromBody] RecipeEntity recipeEntity)
        {
            if (recipeEntity == null) return BadRequest();

            if (recipeEntity.Id == Guid.Empty) recipeEntity.Id = Guid.NewGuid();
            if (recipeEntity.CreatedDate == default) recipeEntity.CreatedDate = DateTimeOffset.UtcNow;

            var rowsAffected = await _recipeRepository.InsertAsync(recipeEntity);

            if (rowsAffected > 0)
            {
                // Return 201 with location header (location uses this action as a simple reference)
                return CreatedAtAction(nameof(AddNewRecipe), new { id = recipeEntity.Id }, recipeEntity);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
