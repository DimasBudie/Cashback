using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Service;
using Microsoft.AspNetCore.Authorization;
using Cashback.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class LoginController : ControllerBase
    {        
        private readonly ILogger _logger;

        public LoginController(ILogger<LoginController> logger){
            _logger = logger;
        }

        /// <summary>
        /// Autentica um usuário e retorna o token necessário para os demais endpoints - Acessível a todos os Usuários.
        /// </summary>
        [HttpPost]             
        [AllowAnonymous]  
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Authenticate([FromServices]IUserService userService, [FromBody]AuthenticateRequest model) 
        {            
            var response = await userService.Authenticate(model);

             if (response == null)
             {
                 _logger.LogInformation($"Usuário ({model.Email}) ou senha incorreta.");
                return BadRequest(new { message = "Username or password is incorrect" });
             }
            
            _logger.LogInformation($"Usuário com E-mail: {model.Email} logou na aplicação.");
            return Ok(response);
        }            
    }
}