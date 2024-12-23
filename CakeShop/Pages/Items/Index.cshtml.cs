﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CakeShop.Data;
using CakeShop.Items;
using Microsoft.AspNetCore.Authorization;

namespace CakeShop.Pages.Items
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CakeShop.Data.ApplicationDbContext _context;

        public IndexModel(CakeShop.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Item> Item { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Item != null)
            {
                Item = await _context.Item.ToListAsync();
            }
        }
    }
}
