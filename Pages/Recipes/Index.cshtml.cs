using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GocBep.Data;
using GocBep.Models;

namespace GocBep.Pages.Recipes
{
    public class IndexModel : PageModel
    {
        private readonly GocBep.Data.ApplicationDbContext _context;

        public IndexModel(GocBep.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Recipe> Recipe { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Recipe = await _context.Recipes
                .Include(r => r.User).ToListAsync();
        }
    }
}
