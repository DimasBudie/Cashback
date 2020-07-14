using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cashback.Models;
using Cashback.Repository;
using Microsoft.AspNetCore.Authorization;
using Cashback.Service;
using System;

namespace Cashback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {

        [HttpGet]
        [Authorize("Usuario")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetAsync([FromServices] IPurchaseService purchaseService)
        {            
            var items = await purchaseService.GetPurchases(User.Identity.Name);

            return items.ToList();
        }

        [HttpGet("GetAll")]
        [Authorize("Administrador")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetAll([FromServices] IPurchaseService purchaseService)
        {            
            var items = await purchaseService.GetPurchases();

            return items.ToList();
        }

        [HttpGet("{id}")]
        [Authorize("All")]
        public async Task<ActionResult<Purchase>> Get([FromServices] IPurchaseRepository purchaseRepository, string id)
        {
            var purchase = await purchaseRepository.GetById(id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        [HttpPost]
        [Authorize("Usuario")]
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

        [HttpPut("{id}")]
        [Authorize("All")]  
        public async Task<IActionResult> Update([FromServices]IPurchaseService purchaseService, string id, Purchase purchase)
        {
            var response = await purchaseService.UpdatePurchase(id, purchase);

           if (response.Invalid)
                return BadRequest(response.GetProblemDetails(response));

            return Created($"{ GetType().Name.Replace("Controller", "").ToLower()}/", response.Value);
        }

        [HttpDelete("{id}")]
        [Authorize("Administrador")]  
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