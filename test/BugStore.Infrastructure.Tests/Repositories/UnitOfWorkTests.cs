using BugStore.Infrastructure.Tests.Builders;
using BugStore.Infra.Repositories;

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods

namespace BugStore.Infrastructure.Tests.Repositories;

public class UnitOfWorkTests : TestBase
{
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _unitOfWork = new UnitOfWork(Context);
    }

    [Fact]
    public async Task CommitAsync_WhenChangesExist_ShouldSaveChangesToDatabase()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        await Context.Customers.AddAsync(customer);

        // Act
        await _unitOfWork.CommitAsync();

        // Assert
        var savedCustomer = await Context.Customers.FindAsync(customer.Id);
        Assert.NotNull(savedCustomer);
        Assert.Equal(customer.Name, savedCustomer.Name);
        Assert.Equal(customer.Email, savedCustomer.Email);
    }

    [Fact]
    public async Task CommitAsync_WhenNoChanges_ShouldNotThrow()
    {
        // Act & Assert
        await _unitOfWork.CommitAsync();
    }

    [Fact]
    public async Task CommitAsync_WithMultipleEntities_ShouldSaveAllChanges()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var product = new ProductBuilder().Build();
        
        await Context.Customers.AddAsync(customer);
        await Context.Products.AddAsync(product);

        // Act
        await _unitOfWork.CommitAsync();

        // Assert
        var savedCustomer = await Context.Customers.FindAsync(customer.Id);
        var savedProduct = await Context.Products.FindAsync(product.Id);
        
        Assert.NotNull(savedCustomer);
        Assert.NotNull(savedProduct);
    }

    [Fact]
    public async Task CommitAsync_WithUpdates_ShouldPersistChanges()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Act
        customer.Update("Updated Name", "updated@test.com", "987654321", customer.BirthDate);
        await _unitOfWork.CommitAsync();

        // Assert
        var updatedCustomer = await Context.Customers.FindAsync(customer.Id);
        Assert.NotNull(updatedCustomer);
        Assert.Equal("Updated Name", updatedCustomer.Name);
        Assert.Equal("updated@test.com", updatedCustomer.Email);
    }

    [Fact]
    public async Task CommitAsync_WithDeletes_ShouldRemoveEntities()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();

        // Act
        Context.Customers.Remove(customer);
        await _unitOfWork.CommitAsync();

        // Assert
        var deletedCustomer = await Context.Customers.FindAsync(customer.Id);
        Assert.Null(deletedCustomer);
    }
}