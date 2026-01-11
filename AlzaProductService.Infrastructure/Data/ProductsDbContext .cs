using AlzaProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlzaProductService.Infrastructure.Data;

public class ProductsDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
        : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(cfg =>
        {
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.Name).IsRequired();
            cfg.Property(x => x.ImgUri).IsRequired();
            cfg.Property(x => x.Price).HasPrecision(18, 2);
        });

    }

}
