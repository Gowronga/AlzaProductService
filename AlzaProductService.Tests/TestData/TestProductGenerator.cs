using AlzaProductService.Domain.Entities;

namespace AlzaProductService.Tests.TestData;

public static class TestProductGenerator
{
    private static readonly string[] ProductNames =
    {
        "Laptop",
        "Phone",
        "Tablet",
        "Monitor",
        "Keyboard"
    };

    public static List<Product> Generate(
        int count,
        int seed = 42)
    {
        var random = new Random(seed);
        var products = new List<Product>();

        for (int i = 0; i < count; i++)
        {
            var name = ProductNames[random.Next(ProductNames.Length)];
            var price = GeneratePrice(random);

            products.Add(new Product(
                Guid.NewGuid(),
                $"{name} {i + 1}",
                "/img/test.png",
                price,
                $"Test description {i + 1}"
            ));
        }

        return products;
    }

    private static decimal GeneratePrice(Random random)
    {
        var euros = random.Next(10, 2000);
        var cents = random.Next(0, 100);
        return euros + cents / 100m;
    }
}