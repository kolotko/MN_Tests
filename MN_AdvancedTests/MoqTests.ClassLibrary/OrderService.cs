namespace MoqTests.ClassLibrary;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public void CreateOrder(Order order)
    {
        if (order.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        _orderRepository.SaveOrder(order);
    }

    public Order FindOrder(int orderId)
    {
        return _orderRepository.GetOrderById(orderId);
    }
}