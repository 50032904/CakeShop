using CakeShop.Data;
using CakeShop.Items;
using CakeShop.Pages.Items;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CakeShop.Test
{
    public class CreateModelTests
    {

        [Fact]
        public async Task OnPostAsync_ReturnsRedirectToPageResult_WhenModelStateIsValid()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Item>>();

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Item).Returns(mockSet.Object);

            var pageModel = new CreateModel(mockContext.Object)
            {
                Item = new Item { Title = "Test", Description = "Example Text", ImageDirectory = "Test.jpg" }
            };

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            mockSet.Verify(m => m.Add(It.IsAny<Item>()), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPageResult_WhenModelStateIsInvalid()
        {

            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            var pageModel = new CreateModel(mockContext.Object)
            {
                Item = new Item { Title = "Test", Description = "Example Text", ImageDirectory = "Test.jpg" }
            };
            pageModel.ModelState.AddModelError("Error", "Model State is Invalid.");

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void CreateModel_ShouldHave_AuthorizeAttribute()
        {
            // Arrange
            var attribute = typeof(CreateModel).GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Act
            bool hasAuthorizeAttribute = attribute.Length > 0;

            // Assert
            Assert.True(hasAuthorizeAttribute, "Create page does not enforce authorization.");
        }

        [Fact]
        public async Task PostAsync_WithSqlInjection_DoesNotDeleteTable()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestSQLDb").Options;

            using var context = new ApplicationDbContext(options);
            var pageModel = new CreateModel(context);

            var testItem = new Item { Title = "Test", Description = "Example Text", ImageDirectory = "Test.jpg" };
            context.Item.Add(testItem);
            await context.SaveChangesAsync();


            // Act
            pageModel.Item = new Item { Title = "'; DROP TABLE Item; --", Description = "'; DROP TABLE Item; --", ImageDirectory = "'; DROP TABLE Item; --" };
            await pageModel.OnPostAsync();

            // Assert
            var itemExists = context.Item.Any();
            Assert.True(itemExists, "The Items table should not be deleted.");
        }
    }
}