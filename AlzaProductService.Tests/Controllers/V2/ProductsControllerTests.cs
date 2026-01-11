using AlzaProductService.Api.Controllers.V2;
using AlzaProductService.Application.Products;
using AlzaProductService.Domain.Entities;
using AlzaProductService.Tests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AlzaProductService.Tests.Controllers.V2;

public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _controller = new ProductsController(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetPaged_Returns_BadRequest_For_Invalid_Page()
    {
        // Act
        var result = await _controller.GetPaged(0, 10);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }


    [Theory]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(50)]
    public async Task GetPaged_Allows_Valid_PageSizes(int pageSize)
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, pageSize))
            .ReturnsAsync(new List<Product>());

        var result = await _controller.GetPaged(1, pageSize);

        result.Result.Should().BeOfType<OkObjectResult>();
    }


    [Fact]
    public async Task GetPaged_Returns_BadRequest_For_Invalid_PageSize()
    {
        var result = await _controller.GetPaged(1, 25);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }


    [Fact]
    public async Task GetPaged_Returns_Products()
    {
        // Arrange
        var products = TestProductGenerator.Generate(10);

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetPaged(page: 1, pageSize: 10);

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
    }

}
