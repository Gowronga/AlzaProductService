using AlzaProductService.Api.Controllers.V1;
using AlzaProductService.Application.Products;
using AlzaProductService.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AlzaProductService.Tests.Controllers.V1;

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
    public async Task GetAll_Returns_Ok_With_Products()
    {
        // Arrange
        var products = new List<Product>
        {
            new(Guid.NewGuid(), "Phone", "/img/phone.png", 999, null)
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, int.MaxValue))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>();
    }

    [Fact]
    public async Task GetById_Returns_NotFound_When_Product_Does_Not_Exist()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
