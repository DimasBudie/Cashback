using System.Threading.Tasks;
using System.Net;
using Xunit;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Cashback.Models;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Cashback.IntegrationTest.Scenarios
{
    public class UserTest
    {
        private readonly string token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI1ZjBlMDFjMjZlNDI1MDNkZTRjNmY2ODkiLCJ1bmlxdWVfbmFtZSI6ImFkbWluQGJvdGljYXJpby5jb20iLCJyb2xlIjoiQWRtaW5pc3RyYWRvciIsIm5iZiI6MTU5NDc3ODEwNSwiZXhwIjoxNTk0NzgxNzA1LCJpYXQiOjE1OTQ3NzgxMDV9.8Wpc4edKsVd6fF4wgjJTigL9_Hf6rM_ZBKgQr2HKga0";
        public UserTest()
        {

        }

        private async Task<IHost> CreateHost()
        {
            var hostBuilder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                // Add TestServer
                webHost.UseTestServer();
                webHost.Configure(app => app.Run(async ctx =>
                    await ctx.Response.WriteAsync("Hello World!")));
            });

            // Build and start the IHost
            var host = await hostBuilder.StartAsync();

            return host;
        }

        [Fact]
        public async Task User_Post_CreateFirstUser()
        {

            var host = await CreateHost();

            var client = host.GetTestClient();

            var parametro = new User
            {
                Cpf = "012345678955",
                Email = "teste@teste.com",
                Name = "FirstUser Test"
            };

            var jsonContent = JsonConvert.SerializeObject(parametro);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new
            MediaTypeHeaderValue("application/json");


            var response = await client.PostAsync("/api/CreateFirstUser", contentString);
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_Post_CreateUser()
        {            
            var host = await CreateHost();

            var client = host.GetTestClient();

            var parametro = new User
            {
                Cpf = "012345678955",
                Email = "teste@teste.com",
                Name = "FirstUser Test",
                Password="password",
                Role="Administrador"
            };

            var jsonContent = JsonConvert.SerializeObject(parametro);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new
            MediaTypeHeaderValue("application/json");
            contentString.Headers.Add("Session-Token", token);


            var response = await client.PostAsync("/api/User", contentString);
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task User_Get_GetUsers()
        {
            var host = await CreateHost();

            var client = host.GetTestClient();                       

            var contentString = new StringContent("", Encoding.UTF8, "application/json");            
            contentString.Headers.Add("Session-Token", token);


            var response = await client.GetAsync("/api/User");
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

}

