using Microsoft.Extensions.DependencyInjection;
using Cashback.Repository;

namespace Cashback.Middlewares
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class RepositoryInjections
    {
        public static void InjectRepository(this IServiceCollection services)
        {
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
        }
    }
}