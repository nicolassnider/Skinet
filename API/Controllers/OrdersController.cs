using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers;

[Authorize]
public class OrdersController(
    ICartService cartService,
    IUnitOfWork unitOfWork,
    ILogger<PaymentsController> logger) : BaseApiController
{
    private readonly string _whSecret;

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
            var productItem = await unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);

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

    [HttpPost]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);
            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                return BadRequest("Invalid event data");
            }
            await HandlePaymentIntentSucceded(intent);
            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error");
        }
    }

    private async Task HandlePaymentIntentSucceded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            var spec = new OrderSpecification(intent.Id, true);
            var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec)
                ?? throw new Exception("Order not found");

            if ((long)order.GetTotal() * 100 != intent.Amount)
            {
                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            await unitOfWork.Complete();
            //todo: signalR
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to construct Stripe event");
            throw new StripeException("Invalid signature");
        }
    }
}
