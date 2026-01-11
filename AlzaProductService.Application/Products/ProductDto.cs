using AlzaProductService.Domain.Entities;

namespace AlzaProductService.Application.Products;

public record ProductDto(
    Guid Id,
    string Name,
    string ImgUri,
    decimal Price,
    string? Description
);


