using AjpopsMarketServer.Enums;

namespace AjpopsMarketServer.Types;

public record UserPayload(UserType User, string? Error = null);

public record MemberPayload(MemberT Member, string? Error = null);

public record ProductPayload(ProductT Product, string? Error = null);

public record CatalogPayload(CatalogT Catalog, string? Error = null);

public record CategoryPayload(CategoryT Category, string? Error = null);

public record OrderPayload(OrderT Order, string? Error = null);
