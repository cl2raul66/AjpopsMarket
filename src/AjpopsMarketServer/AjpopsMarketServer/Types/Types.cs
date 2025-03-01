using AjpopsMarketServer.Enums;

namespace AjpopsMarketServer.Types;

// User Types
public record UserT(
    string Id,
    string Username,
    string Email,
    UserType Type,
    DateTime CreatedAt,
    DateTime? LastLogin,
    bool IsActive
);

public record MemberT(
    string Id,
    string Username,
    string Email,
    UserType Type,
    DateTime CreatedAt,
    DateTime? LastLogin,
    bool IsActive,
    MembershipLevel Level,
    DateTime MemberSince,
    DateTime MembershipExpiresAt,
    bool AutoRenew,
    IReadOnlyList<string> AuthorizedCatalogIds
);

// Product Types
public record ProductT(
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
public record CatalogT(
    string Id,
    string Name,
    string Description,
    bool RequiresMembership,
    MembershipLevel MinimumMembershipLevel,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CreatedBy,
    bool IsActive
);

// Category Types
public record CategoryT(
    string Id,
    string Name,
    string Description,
    string? ParentCategoryId,
    string CatalogId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// Order Types
public record OrderT(
    string Id,
    string UserId,
    OrderStatus Status,
    IReadOnlyList<OrderItemT> Items,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? ShippingAddress,
    string? PaymentId
);

public record OrderItemT(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);
