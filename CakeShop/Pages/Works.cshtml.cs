using CakeShop.Items;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CakeShop.Pages
{
    public class WorksModel : PageModel
    {
        private readonly CakeShop.Data.ApplicationDbContext _context;

        public WorksModel(CakeShop.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Item> Item { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Item != null)
            {
                Item = await _context.Item.ToListAsync();
            }
        }
    }
}
