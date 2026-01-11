using AlzaProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlzaProductService.Infrastructure.Data;

public static class ProductSeedGenerator
{
    private static readonly string[] ProductNames =
    {
        "Laptop",
        "Smartphone",
        "Tablet",
        "Monitor",
        "Keyboard",
        "Mouse",
        "Headphones",
        "Smartwatch",
        "Camera",
        "Printer"
    };

    private static readonly string[] ImageUris =
    {
        "/images/laptop.png",
        "/images/phone.png",
        "/images/tablet.png",
        "/images/monitor.png",
        "/images/keyboard.png"
    };

    /// <summary>
    /// Generates random products.
    /// </summary>
    public static List<Product> Generate(int count, int? randomSeed = null)
    {
        var random = randomSeed.HasValue
            ? new Random(randomSeed.Value)
            : new Random();

        var products = new List<Product>();

        for (int i = 0; i < count; i++)
        {
            var name = ProductNames[random.Next(ProductNames.Length)];
            var imgUri = ImageUris[random.Next(ImageUris.Length)];
            var price = GeneratePrice(random);

            products.Add(new Product(
                Guid.NewGuid(),
                $"{name} {random.Next(100, 999)}",
                imgUri,
                price,
                $"Randomly generated {name.ToLower()}"));
        }

        return products;
    }

    private static decimal GeneratePrice(Random random)
    {
        var euros = random.Next(9, 3000);
        var cents = random.Next(0, 100);

        return euros + cents / 100m;
    }
}

public static class DbSeeder
{
    public static async Task SeedAsync(ProductsDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var products = ProductSeedGenerator.Generate(50);
        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
}