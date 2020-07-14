using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Service;
using Microsoft.AspNetCore.Authorization;
using Cashback.Models.ViewModel;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class LoginController : ControllerBase
    {        

        [HttpPost]             
        [AllowAnonymous]  
        public async Task<IActionResult> Authenticate([FromServices]IUserService userService, [FromBody]AuthenticateRequest model) 
        {
            var response = await userService.Authenticate(model);

             if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }            
    }
}