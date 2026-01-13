using AlzaProductService.Api.GraphQL;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlzaProductService.Tests.GraphQL;

public abstract class GraphQLTestBase
{
    protected async Task<IRequestExecutor> CreateExecutorAsync(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();

        configureServices(services);

        services
            .AddGraphQL()
            .AddQueryType<ProductQueries>();

        var serviceProvider = services.BuildServiceProvider();

        return await serviceProvider
            .GetRequiredService<IRequestExecutorResolver>()
            .GetRequestExecutorAsync();
    }
}
