using Cashback.Extensions;
using Cashback.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Cashback.Middlewares
{
    public static class ApiInjections
    {
        public static IServiceCollection InjectApiServices(this IServiceCollection services)
        {
            services.AddSingleton<JwtConfigurations>();
            services.AddMvc();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddHealthChecks();


            services.InjectApplicationServicesWeb();

            InjectSwagger(services);

            InjectExternalServices(services);

            return services;
        }

        public static void InjectApplicationServicesWeb(this IServiceCollection services)
        {
            services.InjectUsuarioLogado();
            services.AddAuthorizedMvc();
        }

        private static void InjectUsuarioLogado(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
        }

        private static void InjectExternalServices(IServiceCollection services)
        {
            services.AddHttpClient("cashback", client =>
            {
                client.BaseAddress = new Uri("https://mdaqk8ek5j.execute-api.us-east-1.amazonaws.com/");
            });
        }

        private static void InjectSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CashBack - Boticario - API",
                    Version = "v1",
                    Description = "API feita com ASP .NET Core version 3.1",
                    Contact = new OpenApiContact
                    {
                        Name = "Dimas Budie",
                        Email = "dimas_budie@hotmail.com"
                    },
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 4sd56pf4rmzwrujb47fre\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}