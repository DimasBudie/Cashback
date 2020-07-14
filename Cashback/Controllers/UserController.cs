using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Service;
using Cashback.Models;
using Cashback.Repository;
using Microsoft.AspNetCore.Authorization;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    [Authorize("Administrador")]
    public class UserController : ControllerBase
    {        

        [HttpGet]        
        public async Task<IActionResult> Get([FromServices]IUserRepository userRepository) {            
            var response = await userRepository.GetItemsAsync();

             if (response == null)
                return NotFound();

            return Ok(response);
        }      

        [HttpGet("{id}")]
        [Authorize("All")]
        public async Task<IActionResult> Get([FromServices] IUserRepository userRepository, string id)
        {
            var user = await userRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]             
        [Authorize("Administrador")]          
        public async Task<IActionResult> Create([FromServices]IUserService userService, [FromBody]User user) {
            var response = await userService.CreateUser(user);

             if (response.Invalid)
                return BadRequest(response.GetProblemDetails(response));

            return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
        }      

        [HttpPost("CreateFirstUser")]             
        [AllowAnonymous]          
        public async Task<IActionResult> CreateFirstUser([FromServices]IUserService userService, [FromBody]User user) {
            var response = await userService.CreateFirstUser(user);

             if (response.Invalid)
                return BadRequest(response.GetProblemDetails(response));

            return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
        }   

        [HttpGet("GetMyAccumulatedCashback")]   
        [Authorize("Usuario")]     
        public async Task<IActionResult> GetMyAccumulatedCashback([FromServices]IUserService userService, [FromServices]IBoticarioService boticarioService) {
            var userMail = User.Identity.Name;
            var user = await userService.GetByEmail(userMail);

            if(user != null){
                var cashback = await boticarioService.Cashback(user.Cpf);

                return Ok(cashback);
            }             
            return NotFound();
        }

        [HttpGet("GetAccumulatedCashback")]   
        [Authorize("Administrador")]     
        public async Task<IActionResult> GetAccumulatedCashback([FromServices]IUserService userService, [FromServices]IBoticarioService boticarioService, [FromQuery]string cpf) 
        { 
            var cashback = await boticarioService.Cashback(cpf);
            return Ok(cashback);
        }                         
        
    }
}