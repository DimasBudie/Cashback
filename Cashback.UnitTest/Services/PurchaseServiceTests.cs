using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Repository;
using Cashback.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Cashback.UnitTest.Services
{
    [TestFixture()]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class PurchaseServiceTests
    {
        Mock<IUserRepository> userRepository;
        Mock<IPurchaseRepository> purchaseRepository;
        Mock<IConfiguration> configuration;
        Mock<ILogger<PurchaseService>> logger;
        PurchaseService purchaseService;
        User user;
        Purchase purchase;
        Response response;

        [SetUp]
        public void Initialize()
        {
            userRepository = new Mock<IUserRepository>();
            purchaseRepository = new Mock<IPurchaseRepository>();
            logger = new Mock<ILogger<PurchaseService>>();
            configuration = new Mock<IConfiguration>();
            configuration.Setup(t => t["DefautCpf:Cpf"]).Returns("01234567890");
            GlobalSettings.Configuration = configuration.Object;
            purchaseService = new PurchaseService(userRepository.Object, purchaseRepository.Object, logger.Object);
            user = new User { Cpf = "01234567895", Email = "teste@teste.com", Name = "Teste", Role = "Usuario", CreatedAt = new DateTimeOffset(), Id = "5f0e43f02f695b5ae0d8526e", Password = "AQAAAAEAACcQAAAAEFtCyzuAlRazNd7bNQ203nO5lEwGHFBkXP65hIsLQ2kgfk2RWPCxYX4DxSkWsrQ3zQ==" };
            purchase = new Purchase("d4s56", 456, "teste@teste.com", "01234567895");
            response = new Response();
        }

        [Test()]
        public async Task GetPurchasesTest()
        {            
            var purchases = new List<Purchase>();
            purchases.Add(purchase);
            ICollection<Purchase> listPurchases = purchases;
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));
            purchaseRepository.Setup(t => t.GetByCpf("01234567895")).Returns(Task.Run(() => listPurchases));

            var result = await purchaseService.GetPurchases("teste@teste.com");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [Test()]
        public async Task GetPurchasesTest_Fail()
        {
            User userNull = null;
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => userNull));

            var result = await purchaseService.GetPurchases("teste@teste.com");

            Assert.IsNull(result);
        }

        [Test()]
        public async Task GetPurchasesTest_Exception()
        {
            user.Cpf = null;
            userRepository.Setup(t => t.GetByEmail("teste@teste.com")).Returns(Task.Run(() => user));

            
            var result = await purchaseService.GetPurchases("teste@teste.com");

            Assert.IsNull(result);
        }

        [Test()]
        public async Task GetAllPurchasesTest()
        {
            var purchases = new List<Purchase>();
            purchases.Add(purchase);
            ICollection<Purchase> listPurchases = purchases;
            
            purchaseRepository.Setup(t => t.GetItemsAsync()).Returns(Task.Run(() => listPurchases));

            var result = await purchaseService.GetPurchases();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [Test()]
        public async Task GetAllPurchasesTest_Exception()
        {
            userRepository = null;
            user.Cpf = null;
            var result = await purchaseService.GetPurchases();

            Assert.IsNull(result);
        }

        [Test()]
        public async Task CreatePurchaseTest()
        {
            purchaseRepository.Setup(t => t.InsertAsync(purchase)).Returns(Task.Run(() => purchase));

            var result = await purchaseService.CreatePurchase(purchase);

            Assert.IsFalse(result.Invalid);
        }

        [Test()]
        public async Task CreatePurchaseTest_Exception()
        {
            userRepository = null;

            var result = await purchaseService.CreatePurchase(null);

            Assert.IsNotNull(result.Notifications);
        }

        [Test()]
        public async Task UpdatePurchaseTest()
        {
            purchase.Id = "5f0e43f02f695b5ae0d8526e";
            purchaseRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => purchase));
            purchaseRepository.Setup(t => t.UpdateAsync(purchase)).Returns(Task.Run(() => purchase));

            var result = await purchaseService.UpdatePurchase("5f0e43f02f695b5ae0d8526e", purchase);

            Assert.IsFalse(result.Invalid);
        }

        [Test()]
        public async Task UpdatePurchaseTest_Fail()
        {
            Purchase purchaseNull = null;
            purchaseRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => purchaseNull));

            var result = await purchaseService.UpdatePurchase("5f0e43f02f695b5ae0d8526e", purchase);

            Assert.IsNotNull(result.Notifications);
        }

        [Test()]
        public async Task UpdatePurchaseTest_Exception()
        {
            purchase.Id = "5f0e43f02f695b5ae0d8526e";
            purchaseRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => purchase));


            var result = await purchaseService.UpdatePurchase("5f0e43f02f695b5ae0d8526e", null);

            Assert.IsNotNull(result.Notifications);
        }
    }
}