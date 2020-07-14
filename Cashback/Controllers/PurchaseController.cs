using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Models;
using Cashback.Repository;
using Microsoft.AspNetCore.Authorization;
using Cashback.Service;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly ILogger _logger;

        public PurchaseController(ILogger<PurchaseController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Retorna os Pedidos do usuário logado - Acessível apenas a Usuarios(Revendedores).
        /// </summary>
        [HttpGet]
        [Authorize("Usuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetAsync([FromServices] IPurchaseService purchaseService)
        {
            try
            {
                var items = await purchaseService.GetPurchases(User.Identity.Name);
                _logger.LogInformation($"Usuário ({User?.Identity.Name}) buscou pelos pedidos e obteve {items.Count()} resultados.");
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Cria todos os Pedidos cadastrados - Acessível apenas a Administradores.
        /// </summary>
        [HttpGet("GetAll")]
        [Authorize("Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetAll([FromServices] IPurchaseService purchaseService)
        {
            try
            {
                var items = await purchaseService.GetPurchases();
                _logger.LogInformation($"Usuário ({User?.Identity.Name}) executou método GetAll.");
                return items.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Retorna os detalhes de um Pedido esecifico - Acessível a todos os usuários Logados.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Purchase>> Get([FromServices] IPurchaseRepository purchaseRepository, string id)
        {
            try
            {
                var purchase = await purchaseRepository.GetById(id);

                _logger.LogInformation($"Usuário ({User?.Identity.Name}) efetuou uma busca pelo pedido com ID: {id}.");

                if (purchase == null)
                {
                    _logger.LogInformation($"Pedido com ID: {id} não encontrado.");
                    return NotFound();
                }

                return purchase;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Cria um novo Pedido - Acessível apenas a Usuários(Revendedores).
        /// </summary>
        [HttpPost]
        [Authorize("Usuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Purchase>> Create([FromServices] IPurchaseService purchaseService, Purchase purchase)
        {
            try
            {
                await purchaseService.CreatePurchase(purchase);
                _logger.LogInformation($"Usuário {User?.Identity.Name} criou um pedido com código: {purchase.Code}.");
                return purchase;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Atualiza as informações (código, Cpf, Valor, Status[Optional]) de um Pedido - Acessível a todos os Usuários Logados.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromServices] IPurchaseService purchaseService, string id, Purchase purchase)
        {
            try
            {
                var response = await purchaseService.UpdatePurchase(id, purchase);
                if (response.Invalid)
                {
                    _logger.LogWarning($"{response.Notifications.FirstOrDefault().Message}");
                    return BadRequest(response.GetProblemDetails(response));                    
                }
                _logger.LogInformation($"Usuário {User?.Identity.Name} atualizou o pedido com Id: {id}.");
                return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Remove um Pedido da base de dados - Acessível apenas a Administradores.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize("Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromServices] IPurchaseRepository purchaseRepository, string id)
        {
            try
            {
                var purchase = await purchaseRepository.GetById(id);

                if (purchase == null)
                {
                    _logger.LogInformation($"Pedido com ID: {id} não encontrado.");
                    return NotFound();
                }

                await purchaseRepository.DeleteAsync(purchase);
                _logger.LogInformation($"Usuário {User?.Identity.Name} Deletou o pedido com Id: {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }
    }
}