using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Repository;

public interface IProductRepository
{
    Task<ProductT> GetProductByIdAsync(string id);
    Task<IEnumerable<ProductT>> GetProductsAsync(ProductFilterInput? filter);
    Task<IEnumerable<ProductT>> GetProductsByCatalogIdAsync(string catalogId);
    Task<IEnumerable<ProductT>> GetProductsByCategoryIdAsync(string categoryId);
    Task<ProductT> CreateProductAsync(CreateProductInput input);
    Task<ProductT> UpdateProductAsync(UpdateProductInput input);
    Task<ProductT> DeleteProductAsync(string id);
}
