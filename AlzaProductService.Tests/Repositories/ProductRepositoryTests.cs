using AlzaProductService.Infrastructure.Data;
using AlzaProductService.Infrastructure.Repositories;
using AlzaProductService.Tests.Common;
using AlzaProductService.Tests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlzaProductService.Tests.Repositories;

public class ProductRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_Returns_All_Products()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var repository = new ProductRepository(context);

        // Act
        var products = await repository.GetAllAsync(1, 10);

        // Assert
        products.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Product_When_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var repository = new ProductRepository(context);
        var existingProduct = context.Products.First();

        // Act
        var product = await repository.GetByIdAsync(existingProduct.Id);

        // Assert
        product.Should().NotBeNull();
        product!.Name.Should().Be(existingProduct.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Null_When_Not_Exists()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var repository = new ProductRepository(context);

        // Act
        var product = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        product.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Returns_Requested_Page_Size()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ProductsDbContext(options);

        var products = TestProductGenerator.Generate(25);
        context.Products.AddRange(products);
        context.SaveChanges();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetAllAsync(page: 1, pageSize: 10);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(10);
    }
}
