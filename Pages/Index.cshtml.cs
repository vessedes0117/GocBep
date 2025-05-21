using GocBep.Data;
using GocBep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GocBep.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Recipe> Recipes { get; set; } = new List<Recipe>();

        [TempData]
        public string? StatusMessage { get; set; }


        public async Task OnGetAsync()
        {
            var recipesQuery = _context.Recipes
                                      .Include(r => r.User)
                                      .Include(r => r.Ratings)
                                      .OrderByDescending(r => r.CreatedAt);

            Recipes = await recipesQuery.ToListAsync();

            foreach (var recipe in Recipes)
            {
                if (recipe.Ratings != null && recipe.Ratings.Any())
                {
                    recipe.AverageRating = recipe.Ratings.Average(r => r.Score);
                }
                else
                {
                    recipe.AverageRating = 0;
                }
            }

            _logger.LogInformation("Trang chủ đã được tải với {RecipeCount} công thức.", Recipes.Count);
        }
    }
}