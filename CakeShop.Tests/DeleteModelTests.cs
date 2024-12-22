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
using System.Threading;
using Microsoft.AspNetCore.Authorization;

namespace CakeShop.Test
{
    public class DeleteModelTests
    {
        [Fact]
        public async Task OnGetAsync_ItemExists_ReturnsPageResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDelete2Db").Options;

            using var context = new ApplicationDbContext(options);
            var item = new Item { Id = 1 };
            context.Item.Add(item);
            await context.SaveChangesAsync();

            var pageModel = new DeleteModel(context);

            // Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_ItemDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDelete3Db").Options;

            using var context = new ApplicationDbContext(options);
            var pageModel = new DeleteModel(context);

            // Act
            var result = await pageModel.OnGetAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_ItemExists_RemovesItemAndRedirects()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDeleteDb").Options;

            using var context = new ApplicationDbContext(options);
            var item = new Item { Id = 1 };
            context.Item.Add(item);
            await context.SaveChangesAsync();

            var pageModel = new DeleteModel(context);

            // Act
            var result = await pageModel.OnPostAsync(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Empty(context.Item);
        }

        [Fact]
        public async Task OnPostAsync_ItemDoesNotExist_ReturnsRedirectToPageIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options;

            using var context = new ApplicationDbContext(options);
            var pageModel = new DeleteModel(context);

            // Act
            var result = await pageModel.OnPostAsync(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public void DeleteModel_ShouldHave_AuthorizeAttribute()
        {
            // Arrange
            var attribute = typeof(DeleteModel).GetCustomAttributes(typeof(AuthorizeAttribute), true);

            // Act
            bool hasAuthorizeAttribute = attribute.Length > 0;

            // Assert
            Assert.True(hasAuthorizeAttribute, "Delete page does not enforce authorization.");
        }
    }
}
