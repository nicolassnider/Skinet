using Core.Entities;

namespace Core.Interfaces;
public interface ICartService
{
    Task<ShoppingCart?> GetCartAsync(string key);
    Task<ShoppingCart?> SetShoppingCart(ShoppingCart cart);
    Task<bool> DeleteCartAsync(string key);

}
