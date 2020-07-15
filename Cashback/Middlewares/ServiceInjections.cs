using Microsoft.Extensions.DependencyInjection;
using Cashback.Service;

namespace Cashback.Middlewares
{
    public static class ServiceInjections
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IBoticarioService, BoticarioService>();
        }
    }
}