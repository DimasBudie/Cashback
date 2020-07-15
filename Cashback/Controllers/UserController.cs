using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Service;
using Cashback.Models;
using Cashback.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore.Internal;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os usuários cadastrados na base - Acessível apenas a Administradores.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize("Administrador")]
        public async Task<IActionResult> Get([FromServices] IUserRepository userRepository)
        {
            try
            {
                var response = await userRepository.GetItemsAsync();

                if (!response.Any())
                {
                    _logger.LogInformation($"Usuário não encontrado.");
                    return NotFound();
                }
                _logger.LogInformation($"Usuário ({User?.Identity.Name}) buscou pelos usuários e obteve {response.Count} resultados.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
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
            try
            {
                var user = await userRepository.GetById(id);

                if (user == null)
                {
                    _logger.LogInformation($"Usuário com id: {id} não encontrado.");
                    return NotFound();
                }
                _logger.LogInformation($"Usuário ({User?.Identity.Name}) efetuou uma busca pelo usuário com ID: {id}.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
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
            try
            {
                var response = await userService.CreateUser(user);

                if (response.Invalid)
                {
                    var foundProblem = response.GetProblemDetails(response);
                    _logger.LogWarning($"{foundProblem}");
                    return BadRequest(foundProblem);
                }
                _logger.LogInformation($"Usuário ({User?.Identity.Name}) criou uma novo usuário com a permissão: {user.Role} e email: {user.Email}.");
                return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
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
            try
            {
                var response = await userService.CreateFirstUser(user);

                if (response.Invalid)
                {
                    var foundProblem = response.GetProblemDetails(response);
                    _logger.LogWarning($"{foundProblem}");
                    return BadRequest(foundProblem);
                }
                _logger.LogInformation($"Foi solicitado a criação de um novo (primeiro) usuário com o email: {user.Email}.");
                return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
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
            try
            {
                var userMail = User?.Identity.Name;
                var user = await userService.GetByEmail(userMail);

                if (user != null)
                {
                    var cashback = await boticarioService.Cashback(user.Cpf);
                    _logger.LogInformation($"Cashback do usuário {userMail} é de: {cashback}.");

                    return Ok(cashback);
                }
                _logger.LogInformation($"Usuário com e-mail: {userMail} não foi encontrado.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Retorna o credito obtido com Cashback do usuário informado via parametro (CPF) - Acessível apenas a Administradores.
        /// </summary>
        [HttpGet("GetAccumulatedCashback")]
        [Authorize("Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAccumulatedCashback([FromServices] IBoticarioService boticarioService, [FromQuery] string cpf)
        {
            try
            {
                var cashback = await boticarioService.Cashback(cpf);
                _logger.LogInformation($"o Usuário {User?.Identity.Name} realizou uma busca de cashback do usuario com o cpf: {cpf}.");
                return Ok(cashback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

    }
}