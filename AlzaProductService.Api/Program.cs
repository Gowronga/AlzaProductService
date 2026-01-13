using AlzaProductService.Api;
using AlzaProductService.Api.Auth;
using AlzaProductService.Api.GraphQL;
using AlzaProductService.Application.Products;
using AlzaProductService.Infrastructure.Data;
using AlzaProductService.Infrastructure.Repositories;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NLog;
using NLog.Web;
using System.Text;


// Logging
var logger = LogManager.Setup().GetCurrentClassLogger();
NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("nlog.config");


try
{
    var builder = WebApplication.CreateBuilder(args);

    #region Configuration
    var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
    #endregion

    #region Services
    // Authentication & Authorization
    builder.AddAlzaAuthentication();
    builder.Services.AddAuthorization();


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
    builder.AddAlzaSwagger();


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
        .AddAuthorization() // <-- Use Nuget HotChocolate.AspNetCore.Authorization
        .AddQueryType<ProductQueries>()
        .AddMutationType<ProductMutations>()
        .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);


    // Add Alza service
    builder.Services.AddSingleton<JwtTokenService>();
    //builder.Services.AddAlzaServices();

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

    app.UseAuthentication();
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

    logger.Info("Application started...");
    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}