using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Repository;
using Cashback.Security;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cashback.Service
{
    public class PurchaseService : IPurchaseService
    {        
        public IPurchaseRepository _purchaseRepository { get; }
        public IUserRepository _userRepository { get; }

        public PurchaseService(JwtConfigurations configurations, IUserRepository userRepository, IPurchaseRepository purchaseRepository){            
            _purchaseRepository = purchaseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Purchase>> GetPurchases(string email){
            var loggedUser = await _userRepository.GetByEmail(email);
            if(loggedUser != null){
                return await _purchaseRepository.GetByCpf(loggedUser.Cpf);
            }
            return null;
        }

        public async Task<IEnumerable<Purchase>> GetPurchases()
        {            
            return await _purchaseRepository.GetItemsAsync();
        }

        public async Task<Response> CreatePurchase(Purchase purchase)
        {
            var response = new Response();            

            response.AddValue(await _purchaseRepository.InsertAsync(purchase));            
            return response;
        }

        public async Task<Response> UpdatePurchase(string id, Purchase purchase)
        {
            var response = new Response();
            var currentPurchase = await _purchaseRepository.GetById(id);
            if (currentPurchase == null)
            {
                response.AddNotification("Pedido Não Localizado!");
                return response;
            }            
            currentPurchase.UpdateValues(purchase.Code, purchase.Value, purchase.Cpf, purchase.Status);            
            await _purchaseRepository.UpdateAsync(currentPurchase);
            response.AddValue(currentPurchase);

            return response;
        }
    }    
}