using GocBep.Data;
using GocBep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GocBep.Pages.Recipes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Recipe Recipe { get; set; } = default!;

        [BindProperty]
        public int RatingScore { get; set; }

        public bool IsFavoritedByCurrentUser { get; set; }

        private async Task LoadRecipeAndRelatedData(int recipeId)
        {
            Recipe = await _context.Recipes
                                   .Include(r => r.User)
                                   .Include(r => r.Ratings)
                                   // .Include(r => r.FavoritedByUsers) // Có thể không cần nếu truy vấn riêng
                                   .FirstOrDefaultAsync(m => m.Id == recipeId);

            if (Recipe != null)
            {
                if (Recipe.Ratings != null && Recipe.Ratings.Any())
                {
                    Recipe.AverageRating = Recipe.Ratings.Average(r => r.Score);
                }
                else
                {
                    Recipe.AverageRating = 0;
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    IsFavoritedByCurrentUser = await _context.UserFavoriteRecipes
                                                         .AnyAsync(ufr => ufr.RecipeId == Recipe.Id && ufr.UserId == currentUser.Id);
                }
                else
                {
                    IsFavoritedByCurrentUser = false;
                }
            }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await LoadRecipeAndRelatedData(id.Value);

            if (Recipe == null)
            {
                return NotFound();
            }

            return Page();
        }

        [Authorize]
        public async Task<IActionResult> OnPostRateAsync(int id)
        {
            if (RatingScore < 1 || RatingScore > 5)
            {
                ModelState.AddModelError(nameof(RatingScore), "Điểm đánh giá phải từ 1 đến 5 sao.");
            }

            await LoadRecipeAndRelatedData(id);
            if (Recipe == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var existingRating = await _context.Ratings
                                               .FirstOrDefaultAsync(r => r.RecipeId == id && r.UserId == currentUser.Id);

            if (existingRating != null)
            {
                existingRating.Score = RatingScore;
                _context.Ratings.Update(existingRating);
                TempData["SuccessMessage"] = "Đánh giá của bạn đã được cập nhật!";
            }
            else
            {
                var newRating = new Rating
                {
                    RecipeId = id,
                    UserId = currentUser.Id,
                    Score = RatingScore,
                };
                _context.Ratings.Add(newRating);
                TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá công thức này!";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = id });
        }

        [Authorize]
        public async Task<IActionResult> OnPostToggleFavoriteAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var existingFavorite = await _context.UserFavoriteRecipes
                                                 .FirstOrDefaultAsync(ufr => ufr.RecipeId == id && ufr.UserId == currentUser.Id);

            if (existingFavorite != null)
            {
                _context.UserFavoriteRecipes.Remove(existingFavorite);
                TempData["SuccessMessage"] = $"Đã bỏ yêu thích công thức '{recipe.Name}'.";
            }
            else
            {
                var newFavorite = new UserFavoriteRecipe
                {
                    RecipeId = id,
                    UserId = currentUser.Id
                };
                _context.UserFavoriteRecipes.Add(newFavorite);
                TempData["SuccessMessage"] = $"Đã thêm công thức '{recipe.Name}' vào danh sách yêu thích!";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = id });
        }
    }
}