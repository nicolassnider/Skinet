﻿using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentsController(IPaymentService paymentService, IGenericRepository<DeliveryMethod> deliveryMethodRepository) : BaseApiController
{
    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
        if (cart == null) return BadRequest("Problem with Cart");
        return Ok(cart);

    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        var deliveryMethods = await deliveryMethodRepository.ListAllAsync();
        return Ok(deliveryMethods);
    }
}
