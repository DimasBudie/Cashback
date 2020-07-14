using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Repository;
using Cashback.Security;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Cashback.Service
{
    public class PurchaseService : IPurchaseService
    {
        public IPurchaseRepository _purchaseRepository { get; }
        public IUserRepository _userRepository { get; }
        private readonly ILogger _logger;

        public PurchaseService(JwtConfigurations configurations, IUserRepository userRepository, IPurchaseRepository purchaseRepository, ILogger<PurchaseService> logger)
        {
            _purchaseRepository = purchaseRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Purchase>> GetPurchases(string email)
        {
            try
            {
                var loggedUser = await _userRepository.GetByEmail(email);
                if (loggedUser != null)
                {
                    return await _purchaseRepository.GetByCpf(loggedUser.Cpf);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }

        public async Task<IEnumerable<Purchase>> GetPurchases()
        {
            try
            {
                return await _purchaseRepository.GetItemsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Response> CreatePurchase(Purchase purchase)
        {
            var response = new Response();
            try
            {
                response.AddValue(await _purchaseRepository.InsertAsync(purchase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                 response.AddNotification(ex.Message);                
            }
            return response;
        }

        public async Task<Response> UpdatePurchase(string id, Purchase purchase)
        {
            var response = new Response();
            try
            {
                var currentPurchase = await _purchaseRepository.GetById(id);
                if (currentPurchase == null)
                {
                    response.AddNotification("Pedido Não Localizado!");
                    return response;
                }
                currentPurchase.UpdateValues(purchase.Code, purchase.Value, purchase.Cpf, purchase.Status);
                await _purchaseRepository.UpdateAsync(currentPurchase);
                response.AddValue(currentPurchase);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);  
                response.AddNotification(ex.Message);                              
            }
            return response;
        }
    }
}