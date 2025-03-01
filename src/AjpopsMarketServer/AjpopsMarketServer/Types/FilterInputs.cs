using AjpopsMarketServer.Enums;

namespace AjpopsMarketServer.Types;

public record ProductFilterInput(
    string? CatalogId,
    string? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock,
    bool? IsAvailable
);

public record CatalogFilterInput(
    bool? RequiresMembership,
    MembershipLevel? MinimumMembershipLevel,
    bool? IsActive
);

public record OrderFilterInput(
    string? UserId,
    OrderStatus? Status,
    DateTime? FromDate,
    DateTime? ToDate
);
