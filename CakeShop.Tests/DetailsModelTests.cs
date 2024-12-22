using CakeShop.Data;
using CakeShop.Items;
using CakeShop.Pages.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShop.Test
{
    public class DetailsModelTests
    {
        [Fact]
        public async Task OnGetAsync_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            var pageModel = new DetailsModel(mockContext.Object);

            // Act
            var result = await pageModel.OnGetAsync(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options;

            using var context = new ApplicationDbContext(options);
            var pageModel = new DetailsModel(context);

            // Act
            var result = await pageModel.OnGetAsync(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsPage_WhenItemExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Item.Add(new Item { Id = 1 });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var pageModel = new DetailsModel(context);

                // Act
                var result = await pageModel.OnGetAsync(1);

                // Assert
                Assert.IsType<PageResult>(result);
                Assert.NotNull(pageModel.Item);
                Assert.Equal(1, pageModel.Item.Id);
            }
        }

        [Fact]
        public void DetailsModel_ShouldHave_AuthorizeAttribute()
        {
            // Arrange
            var attribute = typeof(DetailsModel).GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Act
            bool hasAuthorizeAttribute = attribute.Length > 0;

            // Assert
            Assert.True(hasAuthorizeAttribute, "Details page does not enforce authorization.");
        }
    }
}
