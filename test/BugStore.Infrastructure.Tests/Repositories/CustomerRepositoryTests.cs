using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Builders;
using BugStore.Infra.Repositories;

namespace BugStore.Infrastructure.Tests.Repositories;

public class CustomerRepositoryTests : TestBase
{
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        _repository = new CustomerRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ShouldReturnCustomer()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(customer.Name, result.Name);
        Assert.Equal(customer.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenCustomersExist_ShouldReturnAllCustomers()
    {
        // Arrange
        var customer1 = new CustomerBuilder().WithEmail("customer1@test.com").Build();
        var customer2 = new CustomerBuilder().WithEmail("customer2@test.com").Build();
        
        await Context.Customers.AddRangeAsync(customer1, customer2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenNoCustomers_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCustomerToDatabase()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();

        // Act
        await _repository.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Assert
        var savedCustomer = await Context.Customers.FindAsync(customer.Id);
        Assert.NotNull(savedCustomer);
        Assert.Equal(customer.Name, savedCustomer.Name);
        Assert.Equal(customer.Email, savedCustomer.Email);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustomerInDatabase()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Act
        customer.Update("Updated Name", "updated@test.com", "987654321", customer.BirthDate);
        await _repository.UpdateAsync(customer);
        await Context.SaveChangesAsync();

        // Assert
        var updatedCustomer = await Context.Customers.FindAsync(customer.Id);
        Assert.NotNull(updatedCustomer);
        Assert.Equal("Updated Name", updatedCustomer.Name);
        Assert.Equal("updated@test.com", updatedCustomer.Email);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_ShouldRemoveFromDatabase()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(customer.Id);
        await Context.SaveChangesAsync();

        // Assert
        var deletedCustomer = await Context.Customers.FindAsync(customer.Id);
        Assert.Null(deletedCustomer);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act & Assert
        await _repository.DeleteAsync(nonExistentId);
        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task EmailExistsAsync_WhenEmailExists_ShouldReturnTrue()
    {
        // Arrange
        var customer = new CustomerBuilder().WithEmail("existing@test.com").Build();
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("existing@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WhenEmailDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.EmailExistsAsync("nonexistent@test.com");

        // Assert
        Assert.False(result);
    }
}