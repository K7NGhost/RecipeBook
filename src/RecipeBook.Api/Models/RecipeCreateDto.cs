using System;
using System.Collections.Generic;

namespace RecipeBook.Api.Models
{
    public class RecipeCreateDto
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        // client sends simple strings for ingredients
        public IList<string>? Ingredients { get; set; } = new List<string>();

        // client sends instructions as a single string
        public string? Instructions { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
    }
}