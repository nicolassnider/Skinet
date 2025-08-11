using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto createOrderDto)
    {
        var email = User.GetEmail();
        var cart = await cartService.GetCartAsync(createOrderDto.CartId);
        if (cart == null) return BadRequest("Cart not found");
        if (cart.PaymentIntentId == null) return BadRequest("No payment intent");

        var items = new List<OrderItem>();
        foreach (var item in cart.Items)
        {
            var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);

            if (productItem == null) return BadRequest("Product not found");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = productItem.Name,
                PictureUrl = productItem.PictureUrl,

            };

            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = item.Price,
                Quantity = item.Quantity
            };

            items.Add(orderItem);

        }
        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>()
            .GetByIdAsync(createOrderDto.DeliveryMethodId);
        if (deliveryMethod == null) return BadRequest("Delivery method not found");

        var order = new Order
        {
            OrderItems = items,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = createOrderDto.ShippingAddress,
            Subtotal = items.Sum(i => i.Price * i.Quantity),
            PaymentSummary = createOrderDto.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email
        };

        unitOfWork.Repository<Order>().Add(order);

        if (await unitOfWork.Complete())
        {
            return order;
        }

        return BadRequest("Problem creating order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetEmail());

        var orders = await unitOfWork.Repository<Order>().ListAsync(spec);

        var ordersToReturn = orders.Select(order => order.ToDto()).ToList();

        return Ok(ordersToReturn);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var spec = new OrderSpecification(User.GetEmail(), id);
        var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        if (order == null) return NotFound();
        return order.ToDto();
    }
}
