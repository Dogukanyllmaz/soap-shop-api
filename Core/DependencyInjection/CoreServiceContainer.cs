using Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Core.DependencyInjection
{
    public static class CoreServiceContainer
    {

        public static IServiceCollection AddCoreServices<TContext>
            (this IServiceCollection services,
            IConfiguration configuration,
            string fileName) where TContext : DbContext
        {
            //Add generic database context
            services.AddDbContext<TContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("soapShopConnection"), sqlserverOption =>
                sqlserverOption.EnableRetryOnFailure()));

            //Add Serilog Logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.txt",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day).CreateLogger();

            //add JWT authentication scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

            return services;
        }

        public static IApplicationBuilder UseCorePolicies(this IApplicationBuilder app)
        {
            //use globalexception
            app.UseMiddleware<GlobalException>();
            //register middleware to block all outsiders API calls
            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }

    }
}
