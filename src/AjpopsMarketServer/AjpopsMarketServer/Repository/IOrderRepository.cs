using AjpopsMarketServer.Enums;
using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Repository;

public interface IOrderRepository
{
    Task<OrderT> GetOrderByIdAsync(string id);
    Task<IEnumerable<OrderT>> GetOrdersAsync(OrderFilterInput? filter);
    Task<IEnumerable<OrderT>> GetOrdersByUserIdAsync(string userId);
    Task<OrderT> CreateOrderAsync(CreateOrderInput input, IProductRepository productRepository);
    Task<OrderT> UpdateOrderStatusAsync(string id, OrderStatus status);
}
