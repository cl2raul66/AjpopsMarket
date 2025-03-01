using AjpopsMarketServer.Repository;
using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Services;

public class CategoryInLiteDbService : ICategoryRepository
{
    public Task<CategoryT> CreateCategoryAsync(CreateCategoryInput input)
    {
        throw new NotImplementedException();
    }

    public Task<CategoryT> DeleteCategoryAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CategoryT>> GetCategoriesByCatalogIdAsync(string catalogId)
    {
        throw new NotImplementedException();
    }

    public Task<CategoryT> GetCategoryByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<CategoryT> UpdateCategoryAsync(UpdateCategoryInput input)
    {
        throw new NotImplementedException();
    }
}
