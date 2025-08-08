using Core.Entities;

namespace Infrastructure.Data;
public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(productsData);
            if (products == null) return;

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        if (!context.DeliveryMethods.Any())
        {
            var deliveryMethodsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");
            var deliveryMethods = System.Text.Json.JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
            if (deliveryMethods == null) return;

            context.DeliveryMethods.AddRange(deliveryMethods);
            await context.SaveChangesAsync();
        }
    }
}
