using System.Collections.Generic;
using System.Threading.Tasks;
using Cashback.Middlewares;
using Cashback.Models;

namespace Cashback.Service
{
    public interface IPurchaseService
    {
        Task<Response> CreatePurchase(Purchase purchase);
        Task<IEnumerable<Purchase>> GetPurchases(string email);
        Task<IEnumerable<Purchase>> GetPurchases();
        Task<Response> UpdatePurchase(string id, Purchase purchase);
    }
}