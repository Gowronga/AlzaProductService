using AlzaProductService.Domain.Entities;

namespace AlzaProductService.Application.Products
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetAllAsync(int page, int pageSize);
        Task<Product?> GetByIdAsync(Guid id);
        Task UpdateAsync(Product product);
    }
}