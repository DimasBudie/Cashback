using Cashback.Middlewares;
using Cashback.Models;
using Cashback.Models.ViewModel;
using Cashback.Repository;
using Cashback.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cashback.Service
{
    public class UserService : IUserService
    {
        private readonly JwtConfigurations jwtConfigurations;
        public IUserRepository _userRepository { get; }
        private readonly ILogger _logger;

        public UserService(JwtConfigurations configurations, IUserRepository userRepository, ILogger<UserService> logger)
        {
            jwtConfigurations = configurations;
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            try
            {
                var user = await _userRepository.GetByEmail(model.Email.ToLower());

                if (user == null || !user.Password.VerifyHashPassword(model.Password)) 
                {
                    _logger.LogInformation($"Falha na autenticação: Senha incorreta - Usuário: {model.Email}");
                    return null;
                }

                var token = jwtConfigurations.GenerateJwtToken(user);
                return new AuthenticateResponse(user, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmail(email.ToLower());

                if (user == null)
                {
                    _logger.LogInformation($"Usuário: {email} não encontrado");
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<Response> CreateUser(User user)
        {
            var response = new Response();
            try
            {
                var userExisit = await _userRepository.GetByEmail(user.Email);
                if (userExisit != null)
                {
                    _logger.LogInformation($"Usuário com e-mail: {user.Email} já está cadastrado!");
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.AddNotification(ex.Message);
            }
            return response;
        }

        public async Task<Response> CreateFirstUser(User user)
        {
            var response = new Response();
            try
            {
                var userExisit = await _userRepository.GetItemsAsync();
                if (userExisit != null && userExisit.Count > 0)
                {
                    _logger.LogInformation($"Já existem usuários cadastrados na base de dados, entre em contato com um administrador para criar um novo usuário!");
                    response.AddNotification("Já existem usuários cadastrados na base de dados, entre em contato com um administrador para criar um novo usuário!");
                    return response;
                }

                user.Role = "Administrador";
                await _userRepository.InsertAsync(user);
                response.AddValue(new UserViewModel(user));
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