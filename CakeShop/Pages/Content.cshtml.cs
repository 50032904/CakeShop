using CakeShop.Items;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CakeShop.Pages
{
    public class ContentModel : PageModel
    {
        private readonly CakeShop.Data.ApplicationDbContext _context;

        public ContentModel(CakeShop.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Item> Item { get; set; } = default!;
        [FromRoute]
        public int Id { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Item != null)
            {
                Item = await _context.Item.ToListAsync();
            }
        }
    }
}
