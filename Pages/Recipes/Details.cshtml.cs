using GocBep.Data;
using GocBep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GocBep.Pages.Recipes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Recipe Recipe { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _context.Recipes
                                   .Include(r => r.User)
                                   .Include(r => r.Ratings)
                                   .FirstOrDefaultAsync(m => m.Id == id);

            if (Recipe == null)
            {
                return NotFound();
            }

            if (Recipe.Ratings != null && Recipe.Ratings.Any())
            {
                Recipe.AverageRating = Recipe.Ratings.Average(r => r.Score);
            }
            else
            {
                Recipe.AverageRating = 0;
            }

            return Page();
        }
    }
}