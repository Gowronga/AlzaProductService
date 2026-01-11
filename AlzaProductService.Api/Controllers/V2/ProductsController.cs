using AlzaProductService.Api.Constants;
using AlzaProductService.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace AlzaProductService.Api.Controllers.V2;

[ApiController]
[Route("api/v2/products")]
[ApiExplorerSettings(GroupName = "v2")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private const int DefaultPageSize = 10;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }


    /// <summary>
    /// Returns paged list of products.
    /// Allowed page sizes: 10, 30, 50.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetPaged([FromQuery] int page = 1, [FromQuery] int? pageSize = null)
    {
        if (page <= 0)
            return BadRequest("Page must be greater than 0.");

        var size = pageSize ?? PaginationDefaults.DefaultPageSize;

        // TODO maybe remove upper limit ? 
        if (!PaginationDefaults.AllowedPageSizes.Contains(size))
        {
            return BadRequest($"Invalid pageSize. Allowed values: {string.Join(", ", PaginationDefaults.AllowedPageSizes)}");
        }

        var products = await _repository.GetAllAsync(page, size);

        var result = products.Select(p =>
            new ProductDto(p.Id, p.Name, p.ImgUri, p.Price, p.Description));

        return Ok(result);
    }
}
