using AlzaProductService.Application.Products;

namespace AlzaProductService.Api.GraphQL;

public class ProductQueries
{
    public Task<IReadOnlyList<Domain.Entities.Product>> GetProducts([Service] IProductRepository repository, int page = 1, int pageSize = 10)
        => repository.GetAllAsync(page, pageSize);

    public Task<Domain.Entities.Product?> GetProductById([Service] IProductRepository repository, Guid id)
        => repository.GetByIdAsync(id);
}

public class ProductMutations
{
    public async Task<bool> UpdateDescription(Guid id, string description, [Service] IProductRepository repository)
    {
        var product = await repository.GetByIdAsync(id);
        if (product is null) return false;

        product.UpdateDescription(description);
        await repository.UpdateAsync(product);
        return true;
    }
}
