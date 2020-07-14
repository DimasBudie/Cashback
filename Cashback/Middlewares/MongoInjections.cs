using Cashback.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cashback.Middlewares
{
    public static class MongoDbInjections
    {
        public static void InjectMongoDbConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoConnection(configuration);
        }

        private static void AddMongoConnection(this IServiceCollection services, IConfiguration configuration)
        {
            if(configuration == null) {
                throw new ArgumentNullException("Arquivo appsettings.json não encontrado!");
            }

            if(configuration["MongoStoreSettings:ConnectionString"] == null) {
                throw new ArgumentNullException("Connection String do Banco de dados nao foi encontrado.\n\n Abra o arquivo appsettings.json " + 
                "e adicione na seguinte estrutura: \n 'MongoStoreSettings':' {\n   'ConnectionString':'---------ConnectionString---------', \n   " +
                "'DatabaseName': '------Nome da Database----'\n   }");
            }

            services.AddSingleton(typeof(MongoDbContext), serviceProvider => 
                new MongoDbContext(configuration["MongoStoreSettings:ConnectionString"], configuration["MongoStoreSettings:DatabaseName"]));
        }
    }
}