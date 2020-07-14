using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Service;
using Cashback.Models;
using Cashback.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        /// <summary>
        /// Lista todos os usuários cadastrados na base - Acessível apenas a Administradores.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize("Administrador")]
        public async Task<IActionResult> Get([FromServices] IUserRepository userRepository)
        {
            var response = await userRepository.GetItemsAsync();

            if (response == null)
                return NotFound();

            return Ok(response);
        }

        /// <summary>
        /// Retorna os dados de determinado usuário - Acessível a todos os usuários logados.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromServices] IUserRepository userRepository, string id)
        {
            var user = await userRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Cria um novo usuário - Acessível apenas a Administradores.
        /// </summary>
        [HttpPost]
        [Authorize("Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromServices] IUserService userService, [FromBody] User user)
        {
            var response = await userService.CreateUser(user);

            if (response.Invalid)
                return BadRequest(response.GetProblemDetails(response));

            return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
        }

        /// <summary>
        /// Cria um novo usuário (Administrador) caso ainda não tenha sido criado nenhum usuário - Acessível a todos os usuário.
        /// </summary>
        [HttpPost("CreateFirstUser")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFirstUser([FromServices] IUserService userService, [FromBody] User user)
        {
            var response = await userService.CreateFirstUser(user);

            if (response.Invalid)
                return BadRequest(response.GetProblemDetails(response));

            return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
        }

        /// <summary>
        /// Retorna o crédito obtido com Cashback do usuario logado - Acessível apenas a Usuario(Revendedor).
        /// </summary>
        [HttpGet("GetMyAccumulatedCashback")]
        [Authorize("Usuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMyAccumulatedCashback([FromServices] IUserService userService, [FromServices] IBoticarioService boticarioService)
        {
            var userMail = User.Identity.Name;
            var user = await userService.GetByEmail(userMail);

            if (user != null)
            {
                var cashback = await boticarioService.Cashback(user.Cpf);

                return Ok(cashback);
            }
            return NotFound();
        }

        /// <summary>
        /// Retorna o credito obtido com Cashback do usuário informado via parametro (CPF) - Acessível apenas a Administradores.
        /// </summary>
        [HttpGet("GetAccumulatedCashback")]
        [Authorize("Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAccumulatedCashback([FromServices] IUserService userService, [FromServices] IBoticarioService boticarioService, [FromQuery] string cpf)
        {
            var cashback = await boticarioService.Cashback(cpf);
            return Ok(cashback);
        }

    }
}