namespace MoqTests.ClassLibrary;

public interface IOrderRepository
{
    void SaveOrder(Order order);
    Order GetOrderById(int orderId);
}