using AlzaProductService.Domain.Entities;
using AlzaProductService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AlzaProductService.Tests.Common;

public static class TestDbContextFactory
{
    public static ProductsDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ProductsDbContext(options);

        var products = ProductSeedGenerator.Generate(count: 50, randomSeed: 1234);

        context.Products.AddRange(products);

        context.SaveChanges();
        return context;
    }
}
