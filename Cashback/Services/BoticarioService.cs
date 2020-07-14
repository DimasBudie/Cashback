using Cashback.Models.ViewModel;
using Cashback.Repository;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Cashback.Service
{
    public class BoticarioService : IBoticarioService
    {        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public IPurchaseRepository _purchaseRepository { get; }
        public IUserRepository _userRepository { get; }

        public BoticarioService(IConfiguration configuration, IHttpClientFactory httpClientFactory){            
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<decimal?> Cashback(string cpf)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["ExternalServiceApi:host"]}v1/cashback?cpf={cpf}");
            request.Headers.Add("token", _configuration["ExternalServiceApi:token"]);

            var client = _httpClientFactory.CreateClient("cashback");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var cashBackResponse = JsonConvert.DeserializeObject<CashbackApiViewModel>(responseString);

                return cashBackResponse.Body.Credit;
            }
            return null;
        }
    }    
}