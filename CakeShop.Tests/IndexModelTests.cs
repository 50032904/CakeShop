using CakeShop.Data;
using CakeShop.Items;
using CakeShop.Pages.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShop.Test
{
    public class IndexModelTests
    {
        [Fact]
        public async Task OnGetAsync_PopulatesThePageModel_WithAListOfItems()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestIndexDb").Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Item.AddRange(
                    new Item { Id = 1, Title = "Test", Description = "Example Text", ImageDirectory = "Test.jpg" },
                    new Item { Id = 2, Title = "Test2", Description = "Example Text2", ImageDirectory = "Test.jpg2" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var pageModel = new IndexModel(context);

                // Act
                await pageModel.OnGetAsync();

                // Assert
                Assert.NotNull(pageModel.Item);
                Assert.IsType<List<Item>>(pageModel.Item);
                Assert.Equal(2, pageModel.Item.Count);
            }
        }

        [Fact]
        public void IndexModel_ShouldHave_AuthorizeAttribute()
        {
            // Arrange
            var attribute = typeof(IndexModel).GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Act
            bool hasAuthorizeAttribute = attribute.Length > 0;

            // Assert
            Assert.True(hasAuthorizeAttribute, "Index page does not enforce authorization.");
        }
    }
}
