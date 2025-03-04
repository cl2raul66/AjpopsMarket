// Ignore Spelling: SKU Jwt

using AjpopsMarketServer.Enums;

namespace AjpopsMarketServer.Models;

#region USERS
public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; 
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserType Type { get; set; } = UserType.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Member : User
{
    public MembershipLevel Level { get; set; } = MembershipLevel.Basic;
    public DateTime MemberSince { get; set; } = DateTime.UtcNow;
    public DateTime MembershipExpiresAt { get; set; }
    public bool AutoRenew { get; set; } = false;
    public string? PaymentMethod { get; set; }
    public List<string> AuthorizedCatalogIds { get; set; } = new List<string>();
}
#endregion

#region PRODUCTS
public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string CatalogId { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new List<string>();
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty; // User ID
}

public class Catalog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiresMembership { get; set; } = false;
    public MembershipLevel MinimumMembershipLevel { get; set; } = MembershipLevel.Basic;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty; // User ID
    public bool IsActive { get; set; } = true;
}

public class Category
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ParentCategoryId { get; set; }
    public string CatalogId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
#endregion

#region SALES
public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? ShippingAddress { get; set; }
    public string? PaymentId { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}
#endregion

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public User? User { get; set; }
    public string? ErrorMessage { get; set; }
}
