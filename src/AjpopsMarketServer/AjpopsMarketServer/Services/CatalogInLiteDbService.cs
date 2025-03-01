using AjpopsMarketServer.Repository;
using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Services;

public class CatalogInLiteDbService : ICatalogRepository
{
    public Task<CatalogT> CreateCatalogAsync(CreateCatalogInput input)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CatalogT>> GetAuthorizedCatalogsAsync(string memberId)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogT> GetCatalogByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CatalogT>> GetCatalogsAsync(CatalogFilterInput? filter)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogT> UpdateCatalogAsync(UpdateCatalogInput input)
    {
        throw new NotImplementedException();
    }
}
