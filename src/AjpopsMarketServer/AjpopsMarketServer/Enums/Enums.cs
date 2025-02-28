namespace AjpopsMarketServer.Enums;

public enum UserType
{
    Normal,
    Member,
    Admin
}

public enum MembershipLevel
{
    Basic,
    Premium,
    Gold,
    Platinum
}

public enum OrderStatus
{
    Created,
    Paid,
    Shipped,
    Delivered,
    Cancelled
}
