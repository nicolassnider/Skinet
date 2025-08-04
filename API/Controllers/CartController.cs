using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController(ICartService cartService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        var cart = await cartService.GetCartAsync(id);
        return Ok(cart ?? new ShoppingCart { Id = id }); //stored in clientside
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart = await cartService.SetShoppingCart(cart);
        if (updatedCart == null) return BadRequest("Problem with Cart");
        return updatedCart;
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCart(string id)
    {
        var result = await cartService.DeleteCartAsync(id);
        if (!result) return BadRequest("Problem with Cart");
        return Ok();
    }


}
