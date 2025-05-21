using GocBep.Data;
using GocBep.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GocBep.Pages.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IdentityUser CurrentUser { get; set; } = default!;

        public IList<Recipe> PostedRecipes { get; set; } = new List<Recipe>();


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            CurrentUser = user;

            PostedRecipes = await _context.Recipes
                                        .Where(r => r.UserId == CurrentUser.Id)
                                        .Include(r => r.Ratings)
                                        .OrderByDescending(r => r.CreatedAt)
                                        .ToListAsync();

            foreach (var recipe in PostedRecipes)
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


            return Page();
        }
    }
}