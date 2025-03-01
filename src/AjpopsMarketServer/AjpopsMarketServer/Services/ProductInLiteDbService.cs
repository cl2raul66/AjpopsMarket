using AjpopsMarketServer.Repository;
using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Services;

public class ProductInLiteDbService : IProductRepository
{
    public Task<ProductT> CreateProductAsync(CreateProductInput input)
    {
        throw new NotImplementedException();
    }

    public Task<ProductT> DeleteProductAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ProductT> GetProductByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductT>> GetProductsAsync(ProductFilterInput? filter)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductT>> GetProductsByCatalogIdAsync(string catalogId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductT>> GetProductsByCategoryIdAsync(string categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<ProductT> UpdateProductAsync(UpdateProductInput input)
    {
        throw new NotImplementedException();
    }
}
