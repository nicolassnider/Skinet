using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.Services;
public class PaymentService(
    IConfiguration config,
    ICartService cartService,
    IUnitOfWork unitOfWork) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        Stripe.StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        var cart = await cartService.GetCartAsync(cartId);

        if (cart == null) return null;

        var shippingPrice = 0m;

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);

            if (deliveryMethod == null) return null;

            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in cart.Items)
        {
            var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return null;
            if (item.Price != productItem.Price) ;
        }

        var service = new Stripe.PaymentIntentService();
        Stripe.PaymentIntent? intent = null;
        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new Stripe.PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * i.Price) + (long)(shippingPrice * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };
            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new Stripe.PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * i.Price) + (long)(shippingPrice * 100),
            };

            intent = await service.UpdateAsync(cart.PaymentIntentId, options);


        }

        await cartService.SetShoppingCart(cart);

        return cart;
    }
}
