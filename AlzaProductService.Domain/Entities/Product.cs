
namespace AlzaProductService.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string ImgUri { get; private set; } = default!;
    public decimal Price { get; private set; }
    public string? Description { get; private set; }

    private Product() { } // EF

    public Product(Guid id, string name, string imgUri, decimal price, string? description)
    {
        Id = id;
        Name = name;
        ImgUri = imgUri;
        Price = price;
        Description = description;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }
}