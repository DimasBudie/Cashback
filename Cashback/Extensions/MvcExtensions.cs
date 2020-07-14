using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Cashback.Security;

namespace Cashback.Extensions
{
    public static class MvcExtensions
    {
        public static void AddAuthorizedMvc(this IServiceCollection services)
        {
            AddJwtAuthorization(services);
            AddMvc(services);
        }

        private static void AddJwtAuthorization(IServiceCollection services)
        {
            var settings = services.BuildServiceProvider().GetRequiredService<JwtConfigurations>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = settings.SigningCredentials.Key
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("All", policy => policy.RequireRole(Roles.OperationalUser, Roles.Administrator));
                options.AddPolicy("Usuario", policy => policy.RequireRole(Roles.OperationalUser));
                options.AddPolicy("Administrador", policy => policy.RequireRole(Roles.Administrator));                
            });
        }

        private static void AddMvc(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        public static class Roles
        {
            public const string OperationalUser = "Usuario";
            public const string Administrator = "Administrador";
        }
    }
}