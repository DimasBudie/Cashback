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

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        /// <summary>
        /// Retorna os Pedidos do usuário logado - Acessível apenas a Usuarios(Revendedores).
        /// </summary>
        [HttpGet]
        [Authorize("Usuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetAsync([FromServices] IPurchaseService purchaseService)
        {            
            var items = await purchaseService.GetPurchases(User.Identity.Name);

            return items.ToList();
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
            var items = await purchaseService.GetPurchases();

            return items.ToList();
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
            var purchase = await purchaseRepository.GetById(id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
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
                return purchase;
            }
            catch (Exception ex)
            {
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
        public async Task<IActionResult> Update([FromServices]IPurchaseService purchaseService, string id, Purchase purchase)
        {
            var response = await purchaseService.UpdatePurchase(id, purchase);

           if (response.Invalid)
                return BadRequest(response.GetProblemDetails(response));

            return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
        }

        /// <summary>
        /// Remove um Pedido da base de dados - Acessível apenas a Administradores.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize("Administrador")]  
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromServices]IPurchaseRepository purchaseRepository, string id)
        {
            var purchase = await purchaseRepository.GetById(id);

            if (purchase == null)
            {
                return NotFound();
            }

            await purchaseRepository.DeleteAsync(purchase);

            return NoContent();
        }
    }
}