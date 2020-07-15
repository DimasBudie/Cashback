using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net;
using Cashback.IntegrationTests.Fictures;
using Xunit;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Cashback.Models;

namespace Cashback.IntegrationTests.Scenarios
{
    public class UserTest
    {
        private readonly TestContext _testContext;
        public UserTest()
        {
            _testContext = new TestContext();
        }

        [Fact]
        public async Task User_Post_CreateFirstUser()
        {

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
            //contentString.Headers.Add("Session-Token", session_token);

            var response = await _testContext.Client.PostAsync("/api/CreateFirstUser", contentString);
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        //[Fact]
        //public async Task Values_GetById_ValuesReturnsOkResponse()
        //{
        //    var response = await _testContext.Client.GetAsync("/api/values/5");
        //    response.EnsureSuccessStatusCode();
        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //}

        //[Fact]
        //public async Task Values_GetById_ReturnsBadRequestResponse()
        //{
        //    var response = await _testContext.Client.GetAsync("/api/values/XXX");
        //    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //}

        //[Fact]
        //public async Task Values_GetById_CorrectContentType()
        //{
        //    var response = await _testContext.Client.GetAsync("/api/values/5");
        //    response.EnsureSuccessStatusCode();
        //    response.Content.Headers.ContentType.ToString().Should().Be("text/plain; charset=utf-8");
        //}
    }
}
