using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cashback.Controllers;
using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Models.ViewModel;
using Cashback.Repository;
using Cashback.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Cashback.UnitTest.Controllers
{
    [TestFixture()]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class UserControllerTests
    {
        private Mock<IUserRepository> userRepository;
        private Mock<IUserService> userService;
        private Mock<ILogger<UserController>> logger;
        private Mock<IBoticarioService> botiService;
        private UserController userController;
        private User user;
        private Response response;
        private ChangePasswordViewmodel changePasswordViewmodel;

        [SetUp]
        public void Initialize()
        {
            userRepository = new Mock<IUserRepository>();
            userService = new Mock<IUserService>();
            logger = new Mock<ILogger<UserController>>();
            botiService = new Mock<IBoticarioService>();
            userController = new UserController(logger.Object);
            user = new User { Cpf = "01234567895", Email = "teste@teste.com", Name = "Teste", Role = "Usuario", CreatedAt = new DateTimeOffset(), Id = "5f0e43f02f695b5ae0d8526e" };
            response = new Response();
            changePasswordViewmodel = new ChangePasswordViewmodel { CurrentPassword = "password", NewPassword = "password", ConfirmNewPassword = "password"};
        }

        [Test()]
        public async Task GetUsersTest()
        {
            var users = new List<User>();
            users.Add(user);
            ICollection<User> userList = users;
            userRepository.Setup(t => t.GetItemsAsync()).Returns(Task.Run(() => userList));

            var result = await userController.GetAsync(userRepository.Object);

            Assert.That(result, Is.InstanceOf<IActionResult>());
            var ok = result;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetUsersTest_Fail()
        {
            var users = new List<User>();            
            ICollection<User> userList = users;
            userRepository.Setup(t => t.GetItemsAsync()).Returns(Task.Run(() => userList));

            var result = await userController.GetAsync(userRepository.Object);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            var ok = result as NotFoundResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetUsersTest_Exception()
        {
            var result = await userController.GetAsync(null);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetUsersByIdTest()
        {

            userRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => user));

            var result = await userController.Get(userRepository.Object, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<IActionResult>());
            var ok = result;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetUsersByIdTest_NotFound()
        {
            User nullUser = null;
            userRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => nullUser));

            var result = await userController.Get(userRepository.Object, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            var ok = result as NotFoundResult; 
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetUsersByIdTest_Exception()
        {
            var result = await userController.Get(null, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task CreateUsersTest()
        {            
            
            userService.Setup(t => t.CreateUser(user)).Returns(Task.Run(() => response));

            var result = await userController.Create(userService.Object, user);

            Assert.That(result, Is.InstanceOf<IActionResult>());
            var ok = result;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task CreateUsersTest_Fail()
        {
            response.AddNotification("Email já cadastrado!");
            userService.Setup(t => t.CreateUser(user)).Returns(Task.Run(() => response));

            var result = await userController.Create(userService.Object, user);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task CreateUsersTest_Exception()
        {
            var result = await userController.Create(null, user);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task CreateFirstUsersTest()
        {
            userService.Setup(t => t.CreateFirstUser(user)).Returns(Task.Run(() => response));

            var result = await userController.CreateFirstUser(userService.Object, user);

            Assert.That(result, Is.InstanceOf<CreatedResult>());
            var ok = result as CreatedResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task CreateFirstUsersTest_Fail()
        {
            response.AddNotification("Já existem usuários cadastrados na base de dados, entre em contato com um administrador para criar um novo usuário!");
            userService.Setup(t => t.CreateFirstUser(user)).Returns(Task.Run(() => response));

            var result = await userController.CreateFirstUser(userService.Object, user);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task CreateFirstUsersTest_Exception()
        {
            var result = await userController.CreateFirstUser(null, user);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetMyAcumulatedCashbackTest()
        {            
            userService.Setup(t => t.GetByEmail(null)).Returns(Task.Run(() => user));
            botiService.Setup(t => t.Cashback("01234567895")).Returns(Task.Run(() => (decimal?)589));

            var result = await userController.GetMyAccumulatedCashback(userService.Object, botiService.Object);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetMyAcumulatedCashbackTest_Fail()
        {
            userService.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));
            botiService.Setup(t => t.Cashback("01234567895")).Returns(Task.Run(() => (decimal?)589));

            var result = await userController.GetMyAccumulatedCashback(userService.Object, botiService.Object);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            var ok = result as NotFoundResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetMyAcumulatedCashbackTest_Exceptionl()
        {            
            botiService.Setup(t => t.Cashback("01234567895")).Returns(Task.Run(() => (decimal?)589));

            var result = await userController.GetMyAccumulatedCashback(null, botiService.Object);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetAcumulatedCashbackTest()
        {            
            botiService.Setup(t => t.Cashback("01234567895")).Returns(Task.Run(() => (decimal?)589));

            var result = await userController.GetAccumulatedCashback(botiService.Object, "01234567895");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetAcumulatedCashbackTest_Fail()
        {            

            var result = await userController.GetAccumulatedCashback(null, "01234567895");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task ChangePasswordTest()
        {            
            userService.Setup(t => t.GetByEmail(null)).Returns(Task.Run(() => user));
            userService.Setup(t => t.ChangePassword(changePasswordViewmodel,null)).Returns(Task.Run(() => response));

            var result = await userController.ChangePassword(userService.Object, changePasswordViewmodel);

            Assert.That(result, Is.InstanceOf<OkResult>());
            var ok = result as OkResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task ChangePasswordTest_Fail()
        {
            changePasswordViewmodel.NewPassword = "paSSword";
            response.AddNotification("A nova senha e a confirmação da nova senha estão difetrentes!");
            userService.Setup(t => t.GetByEmail(null)).Returns(Task.Run(() => user));
            userService.Setup(t => t.ChangePassword(changePasswordViewmodel,null)).Returns(Task.Run(() => response));

            var result = await userController.ChangePassword(userService.Object, changePasswordViewmodel);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task ChangePasswordTest_Exceptionl()
        {            
            userService.Setup(t => t.GetByEmail("")).Returns(Task.Run(() => user));
            userService.Setup(t => t.ChangePassword(changePasswordViewmodel,"")).Returns(Task.Run(() => response));

            var result = await userController.ChangePassword(userService.Object, changePasswordViewmodel);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }
    }
}