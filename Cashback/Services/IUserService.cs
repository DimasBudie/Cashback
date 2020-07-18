using System.Threading.Tasks;
using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Models.ViewModel;

namespace Cashback.Service
{
    public interface IUserService
    {
         Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);      
         Task<User> GetByEmail(string email);
        Task<Response> CreateUser(User user);  
        Task<Response> CreateFirstUser(User user);
        Task<Response> ChangePassword(ChangePasswordViewmodel changePasswordViewmodel, string email);
    }
}