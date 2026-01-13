using AlzaProductService.Application.Products;
using AlzaProductService.Domain.Entities;
using AlzaProductService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlzaProductService.Infrastructure.Repositories;

public class MockProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    //public MockProductRepository()
    //{
    //    var products = ProductSeedGenerator.Generate(count: 50, randomSeed: 1234);

    //    _products = products;
    //}

    public MockProductRepository(List<Product> products)
    {
        _products = products;
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(int page, int pageSize)
        => Task.FromResult<IReadOnlyList<Product>>(_products);

    public Task<Product?> GetByIdAsync(Guid id)
        => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    public Task UpdateAsync(Product product)
        => Task.CompletedTask;
}