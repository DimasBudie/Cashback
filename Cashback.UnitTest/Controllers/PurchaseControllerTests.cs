using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cashback.Controllers;
using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Repository;
using Cashback.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Cashback.UnitTest.Controllers
{   

    [TestFixture()]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class PurchaseControllerTests
    {
        private Mock<IPurchaseRepository> purchaseRepository;
        private Mock<IPurchaseService> purchaseService;
        private Mock<ILogger<PurchaseController>> logger;
        private PurchaseController purchaseController;
        private Purchase purchase;
        private Response response;
        private Mock<IConfiguration> configuration;
        
        [SetUp]
        public void Initialize()
        {
            purchaseRepository = new Mock<IPurchaseRepository>();
            purchaseService = new Mock<IPurchaseService>();
            logger = new Mock<ILogger<PurchaseController>>();            
            purchaseController = new PurchaseController(logger.Object);
            configuration = new Mock<IConfiguration>();
            configuration.Setup(t => t["Configurations:DefautCpf.Cpf"]).Returns("01234567890");
            GlobalSettings.Configuration = configuration.Object;
            purchase = new Purchase("4d5sa4", 456, "teste@teste.com", "01234567890");
            response = new Response();            
        }

        [Test()]
        public async Task GetPurchasesTest()
        {
            var purchases = new List<Purchase>();
            purchases.Add(purchase);
            IEnumerable<Purchase> purchaseList = purchases;
            purchaseService.Setup(t => t.GetPurchases()).Returns(Task.Run(() => purchaseList));
            var result = await purchaseController.GetAsync(purchaseService.Object);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
        }
        

        [Test()]
        public async Task GetPurchasesTest_Exception()
        {
            var result = await purchaseController.GetAsync(null);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetAllTest()
        {
            var purchases = new List<Purchase>();
            purchases.Add(purchase);
            IEnumerable<Purchase> purchaseList = purchases;
            purchaseService.Setup(t => t.GetPurchases()).Returns(Task.Run(() => purchaseList));            
            var result = await purchaseController.GetAll(purchaseService.Object);

            Assert.That(result, Is.InstanceOf<IActionResult>());
            var ok = result;
            Assert.That(ok, Is.Not.Null);
        }


        [Test()]
        public async Task GetAllTest_Exception()
        {
            var result = await purchaseController.GetAll(null);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetPurchaseTest()
        {                        
            purchaseRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => purchase));
            var result = await purchaseController.Get(purchaseRepository.Object, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetPurchaseTest_Fail()
        {
            Purchase purchaseNull = null;
            purchaseRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => purchaseNull));
            var result = await purchaseController.Get(purchaseRepository.Object, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            var ok = result as NotFoundResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task GetAllPurchaseTest_Exception()
        {
            var result = await purchaseController.Get(null, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task CreateTest()
        {
            purchaseService.Setup(t => t.CreatePurchase(purchase)).Returns(Task.Run(() => response));
            var result = await purchaseController.Create(purchaseService.Object, purchase);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
        }


        [Test()]
        public async Task Create_Exception()
        {
            var result = await purchaseController.Create(null, purchase);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task UpdateTest()
        {
            purchaseService.Setup(t => t.UpdatePurchase("5f0e43f02f695b5ae0d8526e", purchase)).Returns(Task.Run(() => response));
            var result = await purchaseController.Update(purchaseService.Object, "5f0e43f02f695b5ae0d8526e", purchase);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task UpdateTest_Fail()
        {
            response.AddNotification("Falha ao Atualizar o Pedido");
            purchaseService.Setup(t => t.UpdatePurchase("5f0e43f02f695b5ae0d8526e", purchase)).Returns(Task.Run(() => response));
            var result = await purchaseController.Update(purchaseService.Object, "5f0e43f02f695b5ae0d8526e", purchase);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task UpdateTest_Exception()
        {
            var result = await purchaseController.Update(null, "5f0e43f02f695b5ae0d8526e", purchase);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task DeleteTest()
        {
            purchaseRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => purchase));
            var result = await purchaseController.Delete(purchaseRepository.Object, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<NoContentResult>());
            var ok = result as NoContentResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task DeleteTest_Fail()
        {
            Purchase purchaseNull = null;
            purchaseRepository.Setup(t => t.GetById("5f0e43f02f695b5ae0d8526e")).Returns(Task.Run(() => purchaseNull));
            var result = await purchaseController.Delete(purchaseRepository.Object, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            var ok = result as NotFoundResult;
            Assert.That(ok, Is.Not.Null);
        }

        [Test()]
        public async Task DeleteTest_Exception()
        {
            var result = await purchaseController.Delete(null, "5f0e43f02f695b5ae0d8526e");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var ok = result as BadRequestObjectResult;
            Assert.That(ok, Is.Not.Null);
        }

    }
}