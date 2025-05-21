using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(ApplicationDbContext context,
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

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Recipe.User");
            ModelState.Remove("Recipe.Ratings");
            ModelState.Remove("Recipe.FavoritedByUsers");
            ModelState.Remove("Recipe.UserId");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            Recipe.UserId = user.Id;
            Recipe.CreatedAt = DateTime.UtcNow;

            if (UploadedImage != null && UploadedImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(UploadedImage.FileName);

                var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "recipes");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedImage.CopyToAsync(fileStream);
                }
                Recipe.ImageUrl = "/images/recipes/" + fileName;
            }
            else
            {
            }


            _context.Recipes.Add(Recipe);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Công thức đã được thêm thành công!";
            return RedirectToPage("./Details", new { id = Recipe.Id });
        }
    }
}