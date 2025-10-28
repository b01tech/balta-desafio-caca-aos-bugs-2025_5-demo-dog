using BugStore.Application.DTOs.Requests.Orders;
using BugStore.Application.Handlers.Orders;
using BugStore.Domain.Interfaces;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BugStore.Application.Tests.Handlers;

public class OrderHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Handler _handler;

    public OrderHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();

        _handler = new Handler(
            _unitOfWorkMock.Object,
            _orderRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _productRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateOrder_WithValidData()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var productId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", DateTime.Now.AddYears(-30));
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m);

        var request = new Create
        {
            CustomerId = customerId,
            Lines = new List<OrderLineRequest>
            {
                new OrderLineRequest { ProductId = productId, Quantity = 2 }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Single(result.Lines);
        Assert.Equal(productId, result.Lines.First().ProductId);
        Assert.Equal(2, result.Lines.First().Quantity);
        Assert.Equal(product.Title, result.Lines.First().ProductTitle);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var request = new Create
        {
            CustomerId = customerId,
            Lines = new List<OrderLineRequest>()
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.Null(result);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", DateTime.Now.AddYears(-30));
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m);
        var order = new Order(customerId, customer, new List<OrderLine>());
        var orderLine = new OrderLine(order.Id, 2, product.Id, product);
        order.AddLine(orderLine);

        var request = new BugStore.Application.DTOs.Requests.Orders.GetById { Id = orderId };

        _orderRepositoryMock
            .Setup(x => x.GetWithOrderLinesAsync(orderId))
            .ReturnsAsync(order);

        // Act
        var result = await _handler.GetByIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(customer.Name, result.CustomerName);
        Assert.Single(result.Lines);
        Assert.Equal(product.Id, result.Lines.First().ProductId);
        Assert.Equal(product.Title, result.Lines.First().ProductTitle);
        Assert.Equal(2, result.Lines.First().Quantity);

        _orderRepositoryMock.Verify(x => x.GetWithOrderLinesAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Orders.GetById { Id = orderId };

        _orderRepositoryMock
            .Setup(x => x.GetWithOrderLinesAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.GetByIdAsync(request);

        // Assert
        Assert.Null(result);

        _orderRepositoryMock.Verify(x => x.GetWithOrderLinesAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders_WhenNoCustomerIdProvided()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", DateTime.Now.AddYears(-30));
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m);
        var order = new Order(customerId, customer, new List<OrderLine>());
        var orderLine = new OrderLine(order.Id, 2, product.Id, product);
        order.AddLine(orderLine);

        var orders = new List<Order> { order };
        var request = new BugStore.Application.DTOs.Requests.Orders.Get
        {
            Page = 1,
            PageSize = 10
        };

        _orderRepositoryMock
            .Setup(x => x.GetAllAsync(request.Page, request.PageSize))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.GetAllAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Page, result.Page);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(orders.Count, result.TotalCount);
        Assert.Single(result.Orders);

        _orderRepositoryMock.Verify(x => x.GetAllAsync(request.Page, request.PageSize), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOrdersByCustomer_WhenCustomerIdProvided()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", DateTime.Now.AddYears(-30));
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m);
        var order = new Order(customerId, customer, new List<OrderLine>());
        var orderLine = new OrderLine(order.Id, 2, product.Id, product);
        order.AddLine(orderLine);

        var orders = new List<Order> { order };
        var request = new BugStore.Application.DTOs.Requests.Orders.Get
        {
            CustomerId = customerId,
            Page = 1,
            PageSize = 10
        };

        _orderRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.GetAllAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Page, result.Page);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(orders.Count, result.TotalCount);
        Assert.Single(result.Orders);

        _orderRepositoryMock.Verify(x => x.GetByCustomerIdAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteOrder_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "11999999999", DateTime.Now.AddYears(-30));
        var order = new Order(customerId, customer, new List<OrderLine>());
        var request = new BugStore.Application.DTOs.Requests.Orders.Delete { Id = orderId };

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        _orderRepositoryMock
            .Setup(x => x.DeleteAsync(orderId))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.DeleteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Order excluído com sucesso", result.Message);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.DeleteAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Orders.Delete { Id = orderId };

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.DeleteAsync(request);

        // Assert
        Assert.Null(result);

        _orderRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _orderRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}