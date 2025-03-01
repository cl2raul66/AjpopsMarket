using AjpopsMarketServer.Repository;
using HotChocolate.Subscriptions;

namespace AjpopsMarketServer.Types;

public class Mutation
{
    // User mutations
    public async Task<UserPayload> CreateUser(
        CreateUserInput input,
        [Service] IUserRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var user = await repository.CreateUserAsync(input);
        await eventSender.SendAsync("UserCreated", user, cancellationToken);
        return new UserPayload(user);
    }

    public async Task<UserPayload> UpdateUser(
        UpdateUserInput input,
        [Service] IUserRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var user = await repository.UpdateUserAsync(input);
        await eventSender.SendAsync("UserUpdated", user, cancellationToken);
        return new UserPayload(user);
    }

    // Member mutations
    public async Task<MemberPayload> CreateMember(
        CreateMemberInput input,
        [Service] IMemberRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var member = await repository.CreateMemberAsync(input);
        await eventSender.SendAsync("MemberCreated", member, cancellationToken);
        return new MemberPayload(member);
    }

    public async Task<MemberPayload> UpdateMember(
        UpdateMemberInput input,
        [Service] IMemberRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var member = await repository.UpdateMemberAsync(input);
        await eventSender.SendAsync("MemberUpdated", member, cancellationToken);
        return new MemberPayload(member);
    }

    public async Task<MemberPayload> AuthorizeCatalogForMember(
        string memberId,
        string catalogId,
        [Service] IMemberRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var member = await repository.AuthorizeCatalogForMemberAsync(memberId, catalogId);
        await eventSender.SendAsync("MemberCatalogAuthorized", member, cancellationToken);
        return new MemberPayload(member);
    }

    public async Task<MemberPayload> RevokeCatalogForMember(
        string memberId,
        string catalogId,
        [Service] IMemberRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var member = await repository.RevokeCatalogForMemberAsync(memberId, catalogId);
        await eventSender.SendAsync("MemberCatalogRevoked", member, cancellationToken);
        return new MemberPayload(member);
    }

    // Product mutations
    public async Task<ProductPayload> CreateProduct(
        CreateProductInput input,
        [Service] IProductRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var product = await repository.CreateProductAsync(input);
        await eventSender.SendAsync("ProductCreated", product, cancellationToken);
        return new ProductPayload(product);
    }

    public async Task<ProductPayload> UpdateProduct(
        UpdateProductInput input,
        [Service] IProductRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var product = await repository.UpdateProductAsync(input);
        await eventSender.SendAsync("ProductUpdated", product, cancellationToken);
        return new ProductPayload(product);
    }

    public async Task<ProductPayload> DeleteProduct(
        string id,
        [Service] IProductRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var product = await repository.DeleteProductAsync(id);
        await eventSender.SendAsync("ProductDeleted", product, cancellationToken);
        return new ProductPayload(product);
    }

    // Catalog mutations
    public async Task<CatalogPayload> CreateCatalog(
        CreateCatalogInput input,
        [Service] ICatalogRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var catalog = await repository.CreateCatalogAsync(input);
        await eventSender.SendAsync("CatalogCreated", catalog, cancellationToken);
        return new CatalogPayload(catalog);
    }

    public async Task<CatalogPayload> UpdateCatalog(
        UpdateCatalogInput input,
        [Service] ICatalogRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var catalog = await repository.UpdateCatalogAsync(input);
        await eventSender.SendAsync("CatalogUpdated", catalog, cancellationToken);
        return new CatalogPayload(catalog);
    }

    // Category mutations
    public async Task<CategoryPayload> CreateCategory(
        CreateCategoryInput input,
        [Service] ICategoryRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var category = await repository.CreateCategoryAsync(input);
        await eventSender.SendAsync("CategoryCreated", category, cancellationToken);
        return new CategoryPayload(category);
    }

    public async Task<CategoryPayload> UpdateCategory(
        UpdateCategoryInput input,
        [Service] ICategoryRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var category = await repository.UpdateCategoryAsync(input);
        await eventSender.SendAsync("CategoryUpdated", category, cancellationToken);
        return new CategoryPayload(category);
    }

    public async Task<CategoryPayload> DeleteCategory(
        string id,
        [Service] ICategoryRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var category = await repository.DeleteCategoryAsync(id);
        await eventSender.SendAsync("CategoryDeleted", category, cancellationToken);
        return new CategoryPayload(category);
    }

    // Order mutations
    public async Task<OrderPayload> CreateOrder(
        CreateOrderInput input,
        [Service] IOrderRepository repository,
        [Service] IProductRepository productRepository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var order = await repository.CreateOrderAsync(input, productRepository);
        await eventSender.SendAsync("OrderCreated", order, cancellationToken);
        return new OrderPayload(order);
    }

    public async Task<OrderPayload> UpdateOrderStatus(
        UpdateOrderStatusInput input,
        [Service] IOrderRepository repository,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var order = await repository.UpdateOrderStatusAsync(input.Id, input.Status);
        await eventSender.SendAsync("OrderStatusUpdated", order, cancellationToken);
        return new OrderPayload(order);
    }
}
