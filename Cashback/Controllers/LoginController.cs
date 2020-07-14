using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Service;
using Microsoft.AspNetCore.Authorization;
using Cashback.Models.ViewModel;
using Microsoft.AspNetCore.Http;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class LoginController : ControllerBase
    {        
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
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }            
    }
}