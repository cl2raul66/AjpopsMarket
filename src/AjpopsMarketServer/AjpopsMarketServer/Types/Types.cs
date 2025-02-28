namespace AjpopsMarketServer.Types;

// User Types
public record UserType(
    string Id,
    string Username,
    string Email,
    UserTypeEnum Type,
    DateTime CreatedAt,
    DateTime? LastLogin,
    bool IsActive
);

public record MemberType(
    string Id,
    string Username,
    string Email,
    UserTypeEnum Type,
    DateTime CreatedAt,
    DateTime? LastLogin,
    bool IsActive,
    MembershipLevelEnum Level,
    DateTime MemberSince,
    DateTime MembershipExpiresAt,
    bool AutoRenew,
    IReadOnlyList<string> AuthorizedCatalogIds
);

// Product Types
public record ProductType(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string SKU,
    int Stock,
    string CatalogId,
    string CategoryId,
    IReadOnlyList<string> ImageUrls,
    bool IsAvailable,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// Catalog Types
public record CatalogType(
    string Id,
    string Name,
    string Description,
    bool RequiresMembership,
    MembershipLevelEnum MinimumMembershipLevel,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CreatedBy,
    bool IsActive
);

// Category Types
public record CategoryType(
    string Id,
    string Name,
    string Description,
    string? ParentCategoryId,
    string CatalogId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// Order Types
public record OrderType(
    string Id,
    string UserId,
    OrderStatusEnum Status,
    IReadOnlyList<OrderItemType> Items,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? ShippingAddress,
    string? PaymentId
);

public record OrderItemType(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);
