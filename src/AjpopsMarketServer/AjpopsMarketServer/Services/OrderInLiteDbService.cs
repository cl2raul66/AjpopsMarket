using AjpopsMarketServer.Enums;
using AjpopsMarketServer.Repository;
using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Services;

public class OrderInLiteDbService : IOrderRepository
{
    public Task<OrderT> CreateOrderAsync(CreateOrderInput input, IProductRepository productRepository)
    {
        throw new NotImplementedException();
    }

    public Task<OrderT> GetOrderByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderT>> GetOrdersAsync(OrderFilterInput? filter)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderT>> GetOrdersByUserIdAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<OrderT> UpdateOrderStatusAsync(string id, OrderStatus status)
    {
        throw new NotImplementedException();
    }
}
