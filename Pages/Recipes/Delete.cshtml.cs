using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GocBep.Data;
using GocBep.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace GocBep.Pages.Recipes
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeleteModel(ApplicationDbContext context,
                           UserManager<IdentityUser> userManager,
                           IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Recipe Recipe { get; set; } = default!;

        public string? ErrorMessage { get; set; }

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
                ErrorMessage = "Bạn không có quyền xóa công thức này.";
                return Page();
            }

            Recipe = recipe;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipeToDelete = await _context.Recipes
                                             .Include(r => r.User)
                                             .FirstOrDefaultAsync(r => r.Id == id);

            if (recipeToDelete == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (recipeToDelete.UserId != currentUser?.Id)
            {
                Recipe = recipeToDelete;
                ErrorMessage = "Bạn không có quyền xóa công thức này.";
                return Page();
            }

            if (!string.IsNullOrEmpty(recipeToDelete.ImageUrl) &&
                !recipeToDelete.ImageUrl.Contains("placeholder"))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, recipeToDelete.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (IOException ex)
                    {
                        TempData["WarningMessage"] = $"Công thức đã được xóa, nhưng có lỗi khi xóa file ảnh: {ex.Message}";
                    }
                }
            }


            _context.Recipes.Remove(recipeToDelete);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Công thức đã được xóa thành công!";
            return RedirectToPage("/Index");
        }
    }
}