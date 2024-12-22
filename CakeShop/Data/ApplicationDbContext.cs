using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CakeShop.Items;

namespace CakeShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public virtual DbSet<Item> Item { get; set; }
        // Other properties and methods
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
            // Parameterless
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("TestDatabase");
            }
        }
    }
}
