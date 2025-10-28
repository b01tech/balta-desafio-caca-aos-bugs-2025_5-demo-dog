using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Builders;
using BugStore.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods

namespace BugStore.Infrastructure.Tests.Repositories;

public class OrderRepositoryTests : TestBase
{
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        _repository = new OrderRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product = new ProductBuilder().Build();
        var orderLine = new OrderLineBuilder().WithProduct(product).Build();
        var order = new OrderBuilder().WithCustomer(customer).AddLine(orderLine).Build();

        await Context.Customers.AddAsync(customer);
        await Context.Products.AddAsync(product);
        await Context.Orders.AddAsync(order);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.CustomerId, result.CustomerId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenOrdersExist_ShouldReturnOrdersWithPagination()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product = new ProductBuilder().Build();
        
        var order1 = new OrderBuilder().WithCustomer(customer).Build();
        var order2 = new OrderBuilder().WithCustomer(customer).Build();
        var order3 = new OrderBuilder().WithCustomer(customer).Build();

        await Context.Customers.AddAsync(customer);
        await Context.Products.AddAsync(product);
        await Context.Orders.AddRangeAsync(order1, order2, order3);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product = new ProductBuilder().Build();
        
        var order1 = new OrderBuilder().WithCustomer(customer).Build();
        var order2 = new OrderBuilder().WithCustomer(customer).Build();
        var order3 = new OrderBuilder().WithCustomer(customer).Build();

        await Context.Customers.AddAsync(customer);
        await Context.Products.AddAsync(product);
        await Context.Orders.AddRangeAsync(order1, order2, order3);
        await Context.SaveChangesAsync();

        // Act
        var page1 = await _repository.GetAllAsync(1, 2);
        var page2 = await _repository.GetAllAsync(2, 2);

        // Assert
        Assert.Equal(2, page1.Count());
        Assert.Single(page2);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_WhenCustomerHasOrders_ShouldReturnCustomerOrders()
    {
        // Arrange
        var customer1 = new CustomerBuilder().WithEmail("customer1@test.com").Build();
        var customer2 = new CustomerBuilder().WithEmail("customer2@test.com").Build();
        var product = new ProductBuilder().Build();
        
        var order1 = new OrderBuilder().WithCustomer(customer1).Build();
        var order2 = new OrderBuilder().WithCustomer(customer1).Build();
        var order3 = new OrderBuilder().WithCustomer(customer2).Build();

        await Context.Customers.AddRangeAsync(customer1, customer2);
        await Context.Products.AddAsync(product);
        await Context.Orders.AddRangeAsync(order1, order2, order3);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCustomerIdAsync(customer1.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, order => Assert.Equal(customer1.Id, order.CustomerId));
    }

    [Fact]
    public async Task GetByCustomerIdAsync_WhenCustomerHasNoOrders_ShouldReturnEmptyCollection()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCustomerIdAsync(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetWithOrderLinesAsync_WhenOrderExists_ShouldReturnOrderWithLines()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product1 = new ProductBuilder().WithTitle("Product 1").WithSlug("product-1").Build();
        var product2 = new ProductBuilder().WithTitle("Product 2").WithSlug("product-2").Build();
        
        var order = new OrderBuilder().WithCustomer(customer).Build();
        var orderLine1 = new OrderLineBuilder().WithOrderId(order.Id).WithProduct(product1).Build();
        var orderLine2 = new OrderLineBuilder().WithOrderId(order.Id).WithProduct(product2).Build();
        
        order.AddLine(orderLine1);
        order.AddLine(orderLine2);

        await Context.Customers.AddAsync(customer);
        await Context.Products.AddRangeAsync(product1, product2);
        await Context.Orders.AddAsync(order);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetWithOrderLinesAsync(order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.NotNull(result.Customer);
        Assert.NotNull(result.Lines);
        Assert.Equal(2, result.Lines.Count);
        Assert.All(result.Lines, line => Assert.NotNull(line.Product));
    }

    [Fact]
    public async Task GetWithOrderLinesAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act
        var result = await _repository.GetWithOrderLinesAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddOrderToDatabase()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product = new ProductBuilder().Build();
        var orderLine = new OrderLineBuilder().WithProduct(product).Build();
        var order = new OrderBuilder().WithCustomer(customer).AddLine(orderLine).Build();

        await Context.Customers.AddAsync(customer);
        await Context.Products.AddAsync(product);

        // Act
        await _repository.AddAsync(order);
        await Context.SaveChangesAsync();

        // Assert
        var savedOrder = await Context.Orders.FindAsync(order.Id);
        Assert.NotNull(savedOrder);
        Assert.Equal(order.CustomerId, savedOrder.CustomerId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrderInDatabase()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product = new ProductBuilder().Build();
        var order = new OrderBuilder().WithCustomer(customer).Build();

        await Context.Customers.AddAsync(customer);
        await Context.Products.AddAsync(product);
        await Context.Orders.AddAsync(order);
        await Context.SaveChangesAsync();

        // Act
        order.SetUpdatedAt();
        await _repository.UpdateAsync(order);
        await Context.SaveChangesAsync();

        // Assert
        var updatedOrder = await Context.Orders.FindAsync(order.Id);
        Assert.NotNull(updatedOrder);
        Assert.NotNull(updatedOrder.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_ShouldRemoveFromDatabase()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product = new ProductBuilder().Build();
        var order = new OrderBuilder().WithCustomer(customer).Build();

        await Context.Customers.AddAsync(customer);
        await Context.Products.AddAsync(product);
        await Context.Orders.AddAsync(order);
        await Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(order.Id);
        await Context.SaveChangesAsync();

        // Assert
        var deletedOrder = await Context.Orders.FindAsync(order.Id);
        Assert.Null(deletedOrder);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act & Assert
        await _repository.DeleteAsync(nonExistentId);
        await Context.SaveChangesAsync();
    }
}