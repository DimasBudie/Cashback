using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    class BoticarioServiceTests
    {
        Mock<IConfiguration> configuration;
        Mock<ILogger<BoticarioService>> logger;
        Mock<IHttpClientFactory> httpClientFactory;
        BoticarioService boticarioService;

        [SetUp]
        public void Initialize()
        {
            httpClientFactory = new Mock<IHttpClientFactory>();
            logger = new Mock<ILogger<BoticarioService>>();
            configuration = new Mock<IConfiguration>();
            configuration.Setup(t => t["ExternalServiceApi:host"]).Returns("https://mdaqk8ek5j.execute-api.us-east-1.amazonaws.com/");
            configuration.Setup(t => t["ExternalServiceApi:token"]).Returns("ZXPURQOARHiMc6Y0flhRC1LVlZQVFRnm");            
            GlobalSettings.Configuration = configuration.Object;
            boticarioService = new BoticarioService(configuration.Object, httpClientFactory.Object, logger.Object);           
        }

        [Test()]
        public async Task CashbackTest()
        {
            var result = await boticarioService.Cashback("01234567895");

            Assert.IsNull(result);
        }
    }
}