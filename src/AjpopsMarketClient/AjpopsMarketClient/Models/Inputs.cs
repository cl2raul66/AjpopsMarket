using AjpopsMarketClient.Enums;

namespace AjpopsMarketClient.Models;

#region USER
public record CreateUserInput(
    string UserName,
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    UserType Type = UserType.Normal
);

public record UpdateUserInput(
    string Id,
    string? UserName,
    string? Email,
    string Password,
    string? FirstName,
    string? LastName,
    UserType Type,
    bool? IsActive
);

public record CreateMemberInput(
    string UserName,
    string Email,
    string Password,
    MembershipLevel Level
);

public record UpdateMemberInput(
    string Id,
    string? UserName,
    string? Email,
    bool? IsActive,
    MembershipLevel? Level,
    DateTime? MembershipExpiresAt,
    bool? AutoRenew
);
#endregion

#region PRODUCT
public record CreateProductInput(
    string Name,
    string Description,
    decimal Price,
    string SKU,
    int Stock,
    string CatalogId,
    string CategoryId,
    List<string> ImageUrls,
    bool IsAvailable
);

public record UpdateProductInput(
    string Id,
    string? Name,
    string? Description,
    decimal? Price,
    string? SKU,
    int? Stock,
    string? CategoryId,
    List<string>? ImageUrls,
    bool? IsAvailable
);

public record CreateCatalogInput(
    string Name,
    string Description,
    bool RequiresMembership,
    MembershipLevel MinimumMembershipLevel
);

public record UpdateCatalogInput(
    string Id,
    string? Name,
    string? Description,
    bool? RequiresMembership,
    MembershipLevel? MinimumMembershipLevel,
    bool? IsActive
);

public record CreateCategoryInput(
    string Name,
    string Description,
    string? ParentCategoryId,
    string CatalogId
);

public record UpdateCategoryInput(
    string Id,
    string? Name,
    string? Description,
    string? ParentCategoryId
);
#endregion

#region SALE
public record CreateOrderInput(
    string UserId,
    List<OrderItemInput> Items,
    string? ShippingAddress,
    string? PaymentId
);

public record OrderItemInput(
    string ProductId,
    int Quantity
);

public record UpdateOrderStatusInput(
    string Id,
    OrderStatus Status
);
#endregion
