using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Repository;

public interface ICatalogRepository
{
    Task<IEnumerable<CatalogT>> GetAuthorizedCatalogsAsync(string memberId);
    Task<CatalogT> GetCatalogByIdAsync(string id);
    Task<IEnumerable<CatalogT>> GetCatalogsAsync(CatalogFilterInput? filter);
    Task<CatalogT> CreateCatalogAsync(CreateCatalogInput input);
    Task<CatalogT> UpdateCatalogAsync(UpdateCatalogInput input);
}
