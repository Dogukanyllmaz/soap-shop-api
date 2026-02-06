using Application.Interfaces;
using Core.DependencyInjection;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Add DbContext configuration
            //Add authentication schemes
            CoreServiceContainer.AddCoreServices<ProductDbContext>(services, configuration, configuration["MySerilog: FileName"]!);

            //create dependency injections for infrastructure services here
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //add infrastructure specific policies here
            //register middleware etc.
            //global exception: handles external errors
            //Listen to Only Api Gateway : blocks all outsider calls
            CoreServiceContainer.UseCorePolicies(app);
            return app;
        }
    }
}
