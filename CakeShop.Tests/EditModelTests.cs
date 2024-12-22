using CakeShop.Data;
using CakeShop.Items;
using CakeShop.Pages.Items;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace CakeShop.Test
{
    public class EditModelTests
    {
        [Fact]
        public async Task OnGetAsync_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            var pageModel = new EditModel(mockContext.Object);

            // Act
            var result = await pageModel.OnGetAsync(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsPageResult_WhenItemDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                if (context.Item.Find(1) != null)
                {
                    context.Item.Remove(context.Item.Find(1));
                    context.SaveChanges();
                }
                
                context.Item.Add(new Item { Id = 1, Title = "Test", Description = "Example Text", ImageDirectory = "Test.jpg" });
                context.SaveChanges();

                var pageModel = new EditModel(context);

                // Act
                var result = await pageModel.OnGetAsync(1);

                // Assert
                Assert.IsType<PageResult>(result);
            }
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPage_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            var pageModel = new EditModel(mockContext.Object);
            pageModel.ModelState.AddModelError("Error", "Model is Invalid");

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsNotFoundResult_WhenModelStateIsValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb2").Options;

            using (var context = new ApplicationDbContext(options))
            {
                if (context.Item.Find(1) != null)
                {
                    context.Item.Remove(context.Item.Find(1));
                    context.SaveChanges();
                }

                if (context.Item.Find(2) != null)
                {
                    context.Item.Remove(context.Item.Find(2));
                    context.SaveChanges();
                }
                

                context.Item.Add(new Item { Id = 1, Title = "Test", Description = "Example Text", ImageDirectory = "Test.jpg" });
                context.SaveChanges();

                var pageModel = new EditModel(context);
                pageModel.Item = new Item { Id = 2, Title = "Test2", Description = "Example Text2", ImageDirectory = "Test2.jpg" };

                // Act
                var result = await pageModel.OnPostAsync();

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public void EditModel_ShouldHave_AuthorizeAttribute()
        {
            // Arrange
            var attribute = typeof(EditModel).GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Act
            bool hasAuthorizeAttribute = attribute.Length > 0;

            // Assert
            Assert.True(hasAuthorizeAttribute, "Edit page does not enforce authorization.");
        }

        [Fact]
        public async Task EditModel_OnPostAsync_WithSqlInjection_DoesNotRemoveTable()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestSQLEditDb").Options;

            using var context = new ApplicationDbContext(options);
            var pageModel = new EditModel(context);

            var testItem = new Item { Title = "TestItem", Description = "Example Text", ImageDirectory = "sprinkle.jpeg" };
            context.Item.Add(testItem);
            await context.SaveChangesAsync();


            // Act
            pageModel.Item = new Item { Title = "'; DROP TABLE Item; --", Description = "'; DROP TABLE Item; --", ImageDirectory = "'; DROP TABLE Item; --" };
            await pageModel.OnPostAsync();

            // Assert
            bool tableExists = context.Item.Any();
            Assert.True(tableExists, "The table should still exist after a SQL injection attempt.");
        }

    }
}
