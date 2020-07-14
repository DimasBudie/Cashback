using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Models.ViewModel;
using Cashback.Repository;
using Cashback.Security;
using System.Threading.Tasks;

namespace Cashback.Service
{
    public class UserService : IUserService
    {
        private readonly JwtConfigurations jwtConfigurations;
        public IUserRepository _userRepository { get; }

        public UserService(JwtConfigurations configurations, IUserRepository userRepository){
            jwtConfigurations = configurations;
            _userRepository = userRepository;
        }
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {            
            var user = await _userRepository.GetByEmail(model.Email.ToLower());
            
            if (user == null) return null;

             if(!user.Password.VerifyHashPassword(model.Password)){
                 return null;
             }

            var token = jwtConfigurations.GenerateJwtToken(user);
            return new AuthenticateResponse(user, token);
        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmail(email.ToLower());

            if (user == null) return null;

            return user;
        }

        public async Task<Response> CreateUser(User user)
        {
            var response = new Response();

            var userExisit = await _userRepository.GetByEmail(user.Email);            
            if(userExisit != null )
            {
                response.AddNotification("Email já cadastrado!");
                return response;
            }
            userExisit = new User
            {
                Name = user.Name,
                Email = user.Email.ToLower(),
                Password = user.Password.ToHashPassword(),
                Role = user.Role,
                Cpf = user.Cpf?.Trim().JustNumber()
            };

            await _userRepository.InsertAsync(userExisit);
            response.AddValue(new UserViewModel(userExisit));
            return response;
        }

        public async Task<Response> CreateFirstUser(User user)
        {            
            var userExisit = await _userRepository.GetItemsAsync();
            if(userExisit != null && userExisit.Count > 0)
            {
                var response = new Response();
                response.AddNotification("Já existem usuários cadastrados na base de dados, entre em contato com um administrador para criar um novo usuário!");
                return response;
            }

            user.Role = "Administrador";
            return await CreateUser(user);                        
        }
    }    
}