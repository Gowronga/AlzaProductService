using AlzaProductService.Application.Products;
using AlzaProductService.Infrastructure.Data;
using AlzaProductService.Infrastructure.Repositories;
using AlzaProductService.Api.GraphQL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

#endregion

#region Services

// Controllers (REST)
builder.Services.AddControllers();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddMvc();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Alza Product Service API",
        Version = "v1"
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Alza Product Service API",
        Version = "v2"
    });

});

// Database (SQL / InMemory switch)
if (useInMemoryDb)
{
    builder.Services.AddDbContext<ProductsDbContext>(options =>
        options.UseInMemoryDatabase("AlzaProductsDb"));

    var mockProducts = ProductSeedGenerator.Generate(50, randomSeed: 1234);

    builder.Services.AddSingleton<IProductRepository>(new MockProductRepository(mockProducts));
}
else
{
    builder.Services.AddDbContext<ProductsDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
}

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<ProductQueries>()
    .AddMutationType<ProductMutations>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

#endregion

var app = builder.Build();

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Alza Product Service v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Alza Product Service v2");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// GraphQL endpoint
app.MapGraphQL("/graphql");

// Seed in-memory database
if (useInMemoryDb)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        await DbSeeder.SeedAsync(db);
    }
}

#endregion

app.Run();