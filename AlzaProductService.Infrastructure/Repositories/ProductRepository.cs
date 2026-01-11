using AlzaProductService.Application.Products;
using AlzaProductService.Domain.Entities;
using AlzaProductService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace AlzaProductService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _db;

    public ProductRepository(ProductsDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(int page, int pageSize)
    {
        return await _db.Products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _db.Products.FindAsync(id);
    }

    public async Task UpdateAsync(Product product)
    {
        await _db.SaveChangesAsync();
    }
}
