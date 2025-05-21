using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GocBep.Data;
using GocBep.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace GocBep.Pages.Recipes
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(ApplicationDbContext context,
                         UserManager<IdentityUser> userManager,
                         IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Recipe Recipe { get; set; } = default!;

        [BindProperty]
        public IFormFile? UploadedImage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                                       .Include(r => r.User)
                                       .FirstOrDefaultAsync(m => m.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (recipe.UserId != currentUser?.Id)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền sửa công thức này.";
                return RedirectToPage("./Details", new { id = recipe.Id });
            }

            Recipe = recipe;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var recipeToUpdate = await _context.Recipes
                                             .Include(r => r.User)
                                             .FirstOrDefaultAsync(r => r.Id == id);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (recipeToUpdate.UserId != currentUser?.Id)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền sửa công thức này.";
                return RedirectToPage("./Details", new { id = recipeToUpdate.Id });
            }

            ModelState.Remove("Recipe.User");
            ModelState.Remove("Recipe.Ratings");
            ModelState.Remove("Recipe.FavoritedByUsers");
            ModelState.Remove("Recipe.UserId");
            ModelState.Remove("Recipe.CreatedAt");


            if (await TryUpdateModelAsync<Recipe>(
                recipeToUpdate,
                "Recipe",
                r => r.Name, r => r.Description, r => r.Ingredients, r => r.Instructions,
                r => r.PrepTime, r => r.CookTime, r => r.Servings))
            {
                if (UploadedImage != null && UploadedImage.Length > 0)
                {
                    if (!string.IsNullOrEmpty(recipeToUpdate.ImageUrl) &&
                        !recipeToUpdate.ImageUrl.Contains("placeholder"))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, recipeToUpdate.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(UploadedImage.FileName);
                    var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "recipes");
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }
                    var newFilePath = Path.Combine(uploadsFolderPath, fileName);

                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await UploadedImage.CopyToAsync(fileStream);
                    }
                    recipeToUpdate.ImageUrl = "/images/recipes/" + fileName;
                }

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Công thức đã được cập nhật thành công!";
                    return RedirectToPage("./Details", new { id = recipeToUpdate.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipeToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Page();
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}