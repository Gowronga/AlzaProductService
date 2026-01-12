using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace AlzaProductService.Api;

public static class SturtUp
{

    const string schemeId = "bearer";
    public static void AddAlzaAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = builder.Configuration.GetSection("Authentication:Jwt");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt["SigningKey"]!))
                };
            });
    }

    public static void AddAlzaSwagger(this WebApplicationBuilder builder)
    {
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


            options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // lowercase per RFC 7235
                BearerFormat = "JWT",
                Description = "JWT Authorization header using Bearer scheme"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });

        });
    }


    public static void AddAlzaServices(this IServiceCollection services)
    {
        var types = GetAllImplementationsOfType(typeof(IAlzaService));

        foreach (var type in types)
        {
            services.AddTransient(type);
        }
    }



    private static List<Type> GetAllImplementationsOfType(Type refType)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetExportedTypes())
            .Where(p => refType.IsAssignableFrom(p) && p.IsClass);

        return types.ToList();
    }

}
