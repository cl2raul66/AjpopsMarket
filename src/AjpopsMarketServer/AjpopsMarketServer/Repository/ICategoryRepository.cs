using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Repository;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryT>> GetCategoriesByCatalogIdAsync(string catalogId);
    Task<CategoryT> GetCategoryByIdAsync(string id);
    Task<CategoryT> CreateCategoryAsync(CreateCategoryInput input);
    Task<CategoryT> UpdateCategoryAsync(UpdateCategoryInput input);
    Task<CategoryT> DeleteCategoryAsync(string id);
}
