using System.Collections.Generic;
using System.Threading.Tasks;
using Cashback.Models;

namespace Cashback.Repository
{
    public interface IPurchaseRepository
    {
        Task<Purchase> GetById(string id);
        Task<ICollection<Purchase>> GetItemsAsync();
        Task<ICollection<Purchase>> GetByEmail(string email);
        Task<Purchase> InsertAsync(Purchase purchase);
        Task<Purchase> UpdateAsync(Purchase purchase);
        Task DeleteAsync(Purchase purchase);
    }
}