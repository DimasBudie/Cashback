using Cashback.Models.ViewModel;
using Cashback.Repository;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System;

namespace Cashback.Service
{
    public class BoticarioService : IBoticarioService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public IPurchaseRepository _purchaseRepository { get; }
        public IUserRepository _userRepository { get; }
        private readonly ILogger _logger;

        public BoticarioService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<BoticarioService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<decimal?> Cashback(string cpf)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["ExternalServiceApi:host"]}v1/cashback?cpf={cpf}");
                request.Headers.Add("token", _configuration["ExternalServiceApi:token"]);

                var client = _httpClientFactory.CreateClient("cashback");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var cashBackResponse = JsonConvert.DeserializeObject<CashbackApiViewModel>(responseString);
                    _logger.LogInformation($"Busca realizada em uma API externa. Cpf: {cpf} - Crédito: {cashBackResponse.Body.Credit}");
                    return cashBackResponse.Body.Credit;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}