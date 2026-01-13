using AlzaProductService.Application.Products;
using AlzaProductService.Domain.Entities;
using FluentAssertions;
using GreenDonut;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AlzaProductService.Tests.GraphQL;

public class ProductQueriesTests : GraphQLTestBase
{
    [Fact]
    public async Task Products_Returns_List_Of_Products()
    {
        // Arrange
        var products = new List<Product>
        {
            new(Guid.NewGuid(), "Laptop", "/img.png", 1000, "Desc"),
            new(Guid.NewGuid(), "Mouse", "/img.png", 20, "Desc")
        };

        var repoMock = new Mock<IProductRepository>();
        repoMock
            .Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(products);

        var executor = await CreateExecutorAsync(services =>
        {
            services.AddSingleton<IProductRepository>(repoMock.Object);
        });

        // Act
        var result = await executor.ExecuteAsync(@"
            query {
              products(page: 1, pageSize: 10) {
                id
                name
                price
              }
            }
        ");

        // Assert
        result.ExpectOperationResult().Errors.Should().BeNullOrEmpty();

        var data = result.ExpectOperationResult()
                         .Data!["products"] as IReadOnlyList<object>;

        data.Should().HaveCount(2);
    }



    [Fact]
    public async Task ProductById_Returns_Product_When_Found()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = new Product(
            productId,
            "Laptop",
            "/img.png",
            1000,
            "Desc");

        var repoMock = new Mock<IProductRepository>();
        repoMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var executor = await CreateExecutorAsync(services =>
        {
            services.AddSingleton<IProductRepository>(repoMock.Object);
        });

        // Act
        var result = await executor.ExecuteAsync(@$"
        query {{
          productById(id: ""{productId}"") {{
            id
            name
            price
          }}
        }}
    ");

        // Assert
        result.ExpectOperationResult().Errors.Should().BeNull();

        var data = result.ExpectOperationResult()
                         .Data!["productById"] as IReadOnlyDictionary<string, object>;

        data.Should().NotBeNull();
        data!["id"].ToString().Should().Be(productId.ToString());
    }

    [Fact]
    public async Task ProductById_Returns_Null_When_Not_Found()
    {
        // Arrange
        var repoMock = new Mock<IProductRepository>();
        repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product?)null);

        var executor = await CreateExecutorAsync(services =>
        {
            services.AddSingleton<IProductRepository>(repoMock.Object);
        });

        // Act
        var result = await executor.ExecuteAsync(@"
                query {
                  productById(id: ""00000000-0000-0000-0000-000000000000"") {
                            id
                  }
                }
                ");

        // Assert
        result.ExpectOperationResult().Errors.Should().BeNull();

        var data = result.ExpectOperationResult().Data!["productById"];
        data.Should().BeNull();
    }



}


