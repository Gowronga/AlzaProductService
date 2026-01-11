using AlzaProductService.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace AlzaProductService.Api.Controllers.V1;

[ApiController]
[Route("api/v1/products")]
[ApiExplorerSettings(GroupName = "v1")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Returns all products (no pagination).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _repository.GetAllAsync(1, int.MaxValue);

        var result = products.Select(p => new ProductDto(p.Id, p.Name, p.ImgUri, p.Price, p.Description));

        return Ok(result);
    }

    /// <summary>
    /// Returns a single product by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        return Ok(new ProductDto(product.Id, product.Name, product.ImgUri, product.Price, product.Description));
    }

    /// <summary>
    /// Updates product description.
    /// </summary>
    [HttpPatch("{id:guid}/description")]
    public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] string description)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            return NotFound();

        product.UpdateDescription(description);
        await _repository.UpdateAsync(product);

        return NoContent();
    }
}