using Core.Entities;

namespace Core.Interfaces;
public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort);
    Task<Product?> GetProductByIdAsync(int id);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();
    void AddProductAsync(Product product);
    void UpdateProductAsync(Product product);
    void DeleteProductAsync(Product product);
    bool ProductExists(int id);
    Task<bool> SaveChangesAsync();
}

