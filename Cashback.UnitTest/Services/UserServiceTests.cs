using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Models.ViewModel;
using Cashback.Repository;
using Cashback.Security;
using Cashback.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Cashback.UnitTest.Services
{
    [TestFixture()]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class UserServiceTests
    {
        JwtConfigurations configurations;
        Mock<IUserRepository> userRepository;
        Mock<ILogger<UserService>> logger;
        Mock<IConfiguration> configuration;
        UserService userService;
        User user;
        private AuthenticateResponse authenticateResponse;
        private AuthenticateRequest authenticateRequest;
        Response response;

        [SetUp]
        public void Initialize()
        {
            userRepository = new Mock<IUserRepository>();
            logger = new Mock<ILogger<UserService>>();
            configuration = new Mock<IConfiguration>();
            configuration.Setup(t => t["JwtSettings:Issuer"]).Returns("self");
            configuration.Setup(t => t["JwtSettings:Audience"]).Returns("https://www.mywebsite.com");
            configuration.Setup(t => t["JwtSettings:ValidForMinutes"]).Returns("60");
            configuration.Setup(t => t["JwtSettings:RefreshTokenValidForMinutes"]).Returns("30");
            configuration.Setup(t => t["JwtSettings:SigningKey"]).Returns("OfED+KgbZxtu4e4+JSQWdtSgTnuNixKy1nMVAEww8QL3IN3idcNgbNDSSaV4491Fo3sq2aGSCtYvekzs7JwXJnNAyvDSJjfK/7M8MpxSMnm1vMscBXyiYFXhGC4wqWlYBE828/5DNyw3QZW5EjD7hvDrY5OlYd4smCTa53helNnJz5NT9HQaDbE2sMwIDAQABAoIBAEs63TvT94njrPDP3A/sfCEXg1F2y0D/PjzUhM1aJGcRiOUXnGlYdViGhLnnJoNZTZm9qI1LT0NWcDA5NmBN6gcrk2EApyTt1D1i4AQ66rYoTF9iEC4Wye28v245BYESA6IIelgIxXGsVyllERsbTkaphzibbYfHmvwMxkn135Zfzd/NOXl/O32vYIomzrNEP+tN2WXhhG8c8+iZ8PErBV3CqrYogYy97d2CeQbXcpd5unPiU4TK0nnzeBAXdgeYuJHFC45YHl9UvShRoe6CHR47ceIGp6WMc5BTyyTkZpctuYJTwaChdj/QuRSkTYmn6jFL+MRfYQJ8VVwSVo5DbkECgYEA4/YIMKcwObYcSuHzgkMwH645CRDoy9M98eptAoNLdJBHYz23U5IbGL1+qHDDCPXxKs9ZG7EEqyWezq42eoFoebLA5O6/xrYXoaeIb094dbCF4D932hAkgAaAZkZVsSiWDCjYSV+JoWX4NVBcIL9yyHRhaaPVULTRbPsZQWq9+hMCgYEA48j4RGO7CaVpgUVobYasJnkGSdhkSCd1VwgvHH3vtuk7/JGUBRaZc0WZGcXkAJXnLh7QnDHOzWASdaxVgnuviaDi4CIkmTCfRqPesgDR2Iu35iQsH7P2/o1pzhpXQS/Ct6J7/GwJTqcXCvp4tfZDbFxS8oewzp4RstILj+pDyWECgYByQAbOy5xB8GGxrhjrOl1OI3V2c8EZFqA/NKy5y6/vlbgRpwbQnbNy7NYj+Y/mV80tFYqldEzQsiQrlei78Uu5YruGgZogL3ccj+izUPMgmP4f6+9XnSuN9rQ3jhy4k4zQP1BXRcim2YJSxhnGV+1hReLknTX2IwmrQxXfUW4xfQKBgAHZW8qSVK5bXWPjQFnDQhp92QM4cnfzegxe0KMWkp+VfRsrw1vXNx");
            configurations = new JwtConfigurations(configuration.Object);
            userService = new UserService(configurations, userRepository.Object, logger.Object);
            user = new User { Cpf = "01234567895", Email = "teste@teste.com", Name = "Teste", Role = "Usuario", CreatedAt = new DateTimeOffset(), Id = "5f0e43f02f695b5ae0d8526e", Password = "AQAAAAEAACcQAAAAEFtCyzuAlRazNd7bNQ203nO5lEwGHFBkXP65hIsLQ2kgfk2RWPCxYX4DxSkWsrQ3zQ==" };
            authenticateResponse = new AuthenticateResponse(user, "5f0e43f02f695b5ae0d8526e");
            authenticateRequest = new AuthenticateRequest { Email = "teste@teste.com", Password = "password" };
            response = new Response();
        }

        [Test()]
        public async Task AuthenticateTest()
        {            
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));

            var result = await userService.Authenticate(authenticateRequest);

            Assert.IsNotNull(result.Token);
            Assert.IsNotNull(result.Email);
        }

        [Test()]
        public async Task AuthenticateTest_Fail()
        {
            user.Password = "";
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));

            var result = await userService.Authenticate(authenticateRequest);

            Assert.IsNull(result);            
        }

        [Test()]
        public async Task AuthenticateTest_Exception()
        {
            user.Password = "";
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));
            authenticateRequest = null;
            var result = await userService.Authenticate(authenticateRequest);

            Assert.IsNull(result);
        }

        [Test()]
        public async Task GetByEmailTest()
        {
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));

            var result = await userService.GetByEmail("teste@teste.com");

            Assert.IsNotNull(result.Name);
            Assert.IsNotNull(result.Email);
        }

        [Test()]
        public async Task GetByEmailTest_Fail()
        {
            user = null;
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));

            var result = await userService.GetByEmail("teste@teste.com");

            Assert.IsNull(result);
        }

        [Test()]
        public async Task GetByEmailTest_Exception()
        {
            userRepository = null;
            var result = await userService.GetByEmail(null);
            Assert.IsNull(result);
        }

        [Test()]
        public async Task CreateUserTest()
        {
            User userNull = null;
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => userNull));
            userRepository.Setup(t => t.InsertAsync(user)).Returns(Task.Run(() => user));

            var result = await userService.CreateUser(user);

            Assert.IsFalse(result.Invalid);
        }

        [Test()]
        public async Task CreateUserTest_Fail()
        {            
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));            

            var result = await userService.CreateUser(user);

            Assert.IsNotNull(response.Notifications);
        }

        [Test()]
        public async Task CreateUserTest_Exception()
        {
            userRepository = null;
            var result = await userService.CreateUser(null);
            Assert.IsNotNull(response.Notifications);
        }

        [Test()]
        public async Task CreateFirstUserTest()
        {
            ICollection<User> userList = null;
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));
            userRepository.Setup(t => t.GetItemsAsync()).Returns(Task.Run(() => userList));            
            userRepository.Setup(t => t.InsertAsync(user)).Returns(Task.Run(() => user));
            

            var result = await userService.CreateFirstUser(user);

            Assert.IsFalse(result.Invalid);
        }

        [Test()]
        public async Task CreateFirstUserTest_Fail()
        {
            var users = new List<User>();
            users.Add(user);
            ICollection<User> userList = users;
            userRepository.Setup(t => t.GetItemsAsync()).Returns(Task.Run(() => userList));

            var result = await userService.CreateFirstUser(user);

            Assert.IsNotNull(response.Notifications);
        }

        [Test()]
        public async Task CreateFirstUserTest_Exception()
        {
            ICollection<User> userList = null;
            userRepository.Setup(t => t.GetItemsAsync()).Returns(Task.Run(() => userList));
            userRepository = null;
            var result = await userService.CreateFirstUser(null);
            Assert.IsNotNull(response.Notifications);
        }
    }
}
/*
 * public async Task<Response> CreateFirstUser(User user)
        {
            var response = new Response();
            try
            {
                var userExisit = await _userRepository.GetItemsAsync();
                if (userExisit != null && userExisit.Count > 0)
                {
                    _logger.LogInformation($"Já existem usuários cadastrados na base de dados, entre em contato com um administrador para criar um novo usuário!");
                    response.AddNotification("Já existem usuários cadastrados na base de dados, entre em contato com um administrador para criar um novo usuário!");
                    return response;
                }

                user.Role = "Administrador";
                return await CreateUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.AddNotification(ex.Message);
            }
            return response;
        }
 
  */
