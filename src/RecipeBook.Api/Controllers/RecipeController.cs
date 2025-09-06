using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.ServiceLibrary.Entities;
using RecipeBook.ServiceLibrary.Repositories;
using RecipeBook.Api.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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

        // GET api/recipe
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var recipes = await _recipeRepository.GetAllAsync();
            return Ok(recipes);
        }

        // GET api/recipe/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null) return NotFound();
            return Ok(recipe);
        }

        // POST api/recipe
        [HttpPost]
        public async Task<IActionResult> AddNewRecipe([FromBody] RecipeCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var id = dto.Id.HasValue && dto.Id.Value != Guid.Empty ? dto.Id.Value : Guid.NewGuid();
            var created = dto.CreatedDate ?? DateTimeOffset.UtcNow;

            var recipeEntity = new RecipeEntity
            {
                Id = id,
                Title = dto.Title ?? "Untitled",
                Description = dto.Description ?? string.Empty,
                CreatedDate = created,
                Ingredients = new List<IngredientEntity>(),
                Instructions = new List<InstructionEntity>()
            };

            // Map simple string ingredients into IngredientEntity records
            if (dto.Ingredients != null)
            {
                int pos = 0;
                foreach (var ing in dto.Ingredients)
                {
                    recipeEntity.Ingredients.Add(new IngredientEntity
                    {
                        RecipeId = id,
                        OrdinalPosition = pos++,
                        Unit = string.Empty,
                        Quantity = 0,
                        Ingredient = ing ?? string.Empty
                    });
                }
            }

            // Map single instruction string into one InstructionEntity (or split by lines if you prefer)
            recipeEntity.Instructions.Add(new InstructionEntity
            {
                RecipeId = id,
                OrdinalPosition = 0,
                Instruction = dto.Instructions ?? string.Empty
            });

            var rowsAffected = await _recipeRepository.InsertAsync(recipeEntity);

            if (rowsAffected > 0)
            {
                // Return 201 with location header
                return CreatedAtAction(nameof(GetById), new { id = recipeEntity.Id }, recipeEntity);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // PUT api/recipe/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeEntity recipeEntity)
        {
            if (recipeEntity == null || recipeEntity.Id != id) return BadRequest();

            if (recipeEntity.CreatedDate == default) recipeEntity.CreatedDate = DateTimeOffset.UtcNow;

            var rowsAffected = await _recipeRepository.UpdateAsync(recipeEntity);

            if (rowsAffected > 0) return NoContent();

            return NotFound();
        }

        // DELETE api/recipe/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRecipe(Guid id)
        {
            var rowsAffected = await _recipeRepository.DeleteAsync(id);
            if (rowsAffected > 0) return NoContent();
            return NotFound();
        }
    }
}
