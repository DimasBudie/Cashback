using System;
using System.Threading.Tasks;
using Cashback.Controllers;
using Cashback.Models;
using Cashback.Models.ViewModel;
using Cashback.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Cashback.UnitTest.Controllers
{
    [TestFixture()]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class LoginControllerTests
    {
        private Mock<IUserService> userService;
        private Mock<ILogger<LoginController>> logger;
        private LoginController loginController;
        private AuthenticateResponse authenticateResponse;
        private AuthenticateRequest authenticateRequest;


        [SetUp]
        public void Initialize()
        {
            userService = new Mock<IUserService>();
            logger = new Mock<ILogger<LoginController>>();            
            loginController = new LoginController(logger.Object);
            var user = new User { Cpf = "01234567895", Email = "teste@teste.com", Name = "Teste", Role = "Usuario", CreatedAt = new DateTimeOffset(), Id = "5f0e43f02f695b5ae0d8526e" };
            authenticateResponse = new AuthenticateResponse(user, "5f0e43f02f695b5ae0d8526e");
            authenticateRequest = new AuthenticateRequest { Email = "teste@teste.com", Password = "password" };
        }

        [Test()]
        public async Task AuthenticateTest()
        {
            userService.Setup(t => t.Authenticate(authenticateRequest)).Returns(Task.Run(() => authenticateResponse));

            var result = await loginController.Authenticate(userService.Object, authenticateRequest);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task Authenticate_Fail()
        {
            AuthenticateResponse response = null;
            userService.Setup(t => t.Authenticate(authenticateRequest)).Returns(Task.Run(() => response));

            var result = await loginController.Authenticate(userService.Object, authenticateRequest);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }
    }
}
