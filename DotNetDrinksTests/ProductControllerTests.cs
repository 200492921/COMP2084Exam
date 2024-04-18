using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetDrinks.Controllers;
using DotNetDrinks.Data;
using DotNetDrinks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotNetDrinksTests;

public class ProductsControllerTests
{
    private readonly ApplicationDbContext _context;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb" + Random.Shared.NextInt64()) // Make sure each test method has a unique name for 'databaseName' if they run in parallel
            .Options;
        
        _context = new ApplicationDbContext(options);
        _context.AddRange(
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" }
        );
        _context.SaveChanges();
    }
    
    [Fact]
    public async Task Edit_ReturnsViewResult_WithValidId()
    {
        // Arrange
        var controller = new ProductsController(_context);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model); // Ensure a model is returned
        Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || viewResult.ViewName == "Edit"); // View name is either not set or explicitly set to "Edit"
    }
    
    [Fact]
    public async Task DeleteConfirmed_RemovesProduct_FromDatabase()
    {
        // Arrange
        var controller = new ProductsController(_context);
        var initialCount = _context.Products.Count();

        // Act
        await controller.DeleteConfirmed(1);

        // Assert
        var product = await _context.Products.FindAsync(1);
        var finalCount = _context.Products.Count();
        Assert.Null(product);
        Assert.Equal(initialCount - 1, finalCount); // Check if the count of products is reduced by 1
    }
}
