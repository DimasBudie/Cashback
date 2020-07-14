using System.Collections.Generic;
using System.Threading.Tasks;
using Cashback.Models;

namespace Cashback.Repository
{
    public interface IUserRepository
    {
        Task<User> GetById(string id);
        Task<ICollection<User>> GetItemsAsync();
        Task<User> InsertAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<User> GetByEmail(string email);
    }
}