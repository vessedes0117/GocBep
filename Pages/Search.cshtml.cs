using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GocBep.Data;
using GocBep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GocBep.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thuộc tính để bind với query từ URL (?query=...)
        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        public IList<Recipe> SearchResults { get; set; } = new List<Recipe>();

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Query))
            {
                // Tìm kiếm trong tên món ăn (không phân biệt hoa thường)
                // và có thể mở rộng tìm trong Description nếu muốn
                SearchResults = await _context.Recipes
                    .Include(r => r.User) // Để hiển thị người đăng
                    .Include(r => r.Ratings) // Để tính điểm trung bình
                    .Where(r => r.Name.ToLower().Contains(Query.ToLower()))
                    // Ví dụ mở rộng tìm trong mô tả:
                    // .Where(r => r.Name.ToLower().Contains(Query.ToLower()) || 
                    //              (r.Description != null && r.Description.ToLower().Contains(Query.ToLower())))
                    .OrderByDescending(r => r.CreatedAt) // Hoặc sắp xếp theo mức độ liên quan (phức tạp hơn)
                    .ToListAsync();

                foreach (var recipe in SearchResults)
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
            }
            // Nếu Query rỗng hoặc null, SearchResults sẽ là danh sách rỗng
        }
    }
}