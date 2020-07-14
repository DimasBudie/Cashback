using Microsoft.Extensions.DependencyInjection;
using Cashback.Service;

namespace Cashback.Middlewares
{
    public static class ServiceInjections
    {
        public static void InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IBoticarioService, BoticarioService>();
        }
    }
}