using AjpopsMarketServer.Enums;
using AjpopsMarketServer.Repository;

namespace AjpopsMarketServer.Types;

[QueryType]
public class Query
{
    // User queries
    public async Task<UserType> GetUserById(string id, [Service] IUserRepository repository)
        => await repository.GetUserByIdAsync(id);

    public async Task<IEnumerable<UserType>> GetUsers([Service] IUserRepository repository)
        => await repository.GetAllUsersAsync();

    // Member queries
    public async Task<MemberT> GetMemberById(string id, [Service] IMemberRepository repository)
    => await repository.GetMemberByIdAsync(id);

    public async Task<IEnumerable<MemberT>> GetMembers([Service] IMemberRepository repository)
        => await repository.GetAllMembersAsync();

    // Product queries
    public async Task<ProductT> GetProductById(string id, [Service] IProductRepository repository)
        => await repository.GetProductByIdAsync(id);

    public async Task<IEnumerable<ProductT>> GetProducts(ProductFilterInput? filter, [Service] IProductRepository repository)
        => await repository.GetProductsAsync(filter);

    public async Task<IEnumerable<ProductT>> GetProductsByCatalogId(string catalogId, [Service] IProductRepository repository)
        => await repository.GetProductsByCatalogIdAsync(catalogId);

    public async Task<IEnumerable<ProductT>> GetProductsByCategoryId(string categoryId, [Service] IProductRepository repository)
        => await repository.GetProductsByCategoryIdAsync(categoryId);

    // Catalog queries
    public async Task<CatalogT> GetCatalogById(string id, [Service] ICatalogRepository repository)
        => await repository.GetCatalogByIdAsync(id);

    public async Task<IEnumerable<CatalogT>> GetCatalogs(CatalogFilterInput? filter, [Service] ICatalogRepository repository)
        => await repository.GetCatalogsAsync(filter);

    public async Task<IEnumerable<CatalogT>> GetAuthorizedCatalogs(string memberId, [Service] ICatalogRepository repository)
        => await repository.GetAuthorizedCatalogsAsync(memberId);

    // Category queries
    public async Task<CategoryT> GetCategoryById(string id, [Service] ICategoryRepository repository)
        => await repository.GetCategoryByIdAsync(id);

    public async Task<IEnumerable<CategoryT>> GetCategories(string catalogId, [Service] ICategoryRepository repository)
        => await repository.GetCategoriesByCatalogIdAsync(catalogId);

    // Order queries
    public async Task<OrderT> GetOrderById(string id, [Service] IOrderRepository repository)
        => await repository.GetOrderByIdAsync(id);

    public async Task<IEnumerable<OrderT>> GetOrders(OrderFilterInput? filter, [Service] IOrderRepository repository)
        => await repository.GetOrdersAsync(filter);

    public async Task<IEnumerable<OrderT>> GetUserOrders(string userId, [Service] IOrderRepository repository)
        => await repository.GetOrdersByUserIdAsync(userId);
}
