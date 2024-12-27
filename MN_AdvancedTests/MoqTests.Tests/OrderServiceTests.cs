using Moq;
using MoqTests.ClassLibrary;
using Xunit;

namespace MoqTests.Tests;

public class OrderServiceTests
{
    private readonly OrderService _service;
    private Mock<IOrderRepository> _mockRepository;

    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _service = new OrderService(_mockRepository.Object);
    }
    [Fact]
    public void CreateOrder_ShouldSaveOrder_WhenOrderIsValid()
    {
        // Arrange
        var order = new Order { Id = 1, ProductName = "Laptop", Quantity = 2 };

        // Act
        _service.CreateOrder(order);

        // Assert
        _mockRepository.Verify(repo => repo.SaveOrder(order), Times.Once);
    }

    [Fact]
    public void CreateOrder_ShouldThrowException_WhenQuantityIsZeroOrLess()
    {
        // Arrange
        var order = new Order { Id = 1, ProductName = "Laptop", Quantity = 0 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.CreateOrder(order));
        _mockRepository.Verify(repo => repo.SaveOrder(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public void FindOrder_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var order = new Order { Id = 1, ProductName = "Laptop", Quantity = 2 };
        _mockRepository.Setup(repo => repo.GetOrderById(1)).Returns(order);

        // Act
        var result = _service.FindOrder(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.ProductName, result.ProductName);
        _mockRepository.Verify(repo => repo.GetOrderById(1), Times.Once);
    }
}