using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Builders;
using BugStore.Infra.Repositories;

#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods

namespace BugStore.Infrastructure.Tests.Repositories;

public class ProductRepositoryTests : TestBase
{
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _repository = new ProductRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        await Context.Products.AddAsync(product);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Title, result.Title);
        Assert.Equal(product.Price, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ShouldReturnAllProducts()
    {
        // Arrange
        var product1 = new ProductBuilder().WithTitle("Product 1").WithSlug("product-1").Build();
        var product2 = new ProductBuilder().WithTitle("Product 2").WithSlug("product-2").Build();
        
        await Context.Products.AddRangeAsync(product1, product2);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProducts_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        var product = new ProductBuilder().Build();

        // Act
        await _repository.AddAsync(product);
        await Context.SaveChangesAsync();

        // Assert
        var savedProduct = await Context.Products.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal(product.Title, savedProduct.Title);
        Assert.Equal(product.Price, savedProduct.Price);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProductInDatabase()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        await Context.Products.AddAsync(product);
        await Context.SaveChangesAsync();

        // Act
        product.Update("Updated Title", "Updated Description", "updated-slug", 29.99m);
        await _repository.UpdateAsync(product);
        await Context.SaveChangesAsync();

        // Assert
        var updatedProduct = await Context.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal("Updated Title", updatedProduct.Title);
        Assert.Equal("Updated Description", updatedProduct.Description);
        Assert.Equal(29.99m, updatedProduct.Price);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ShouldRemoveFromDatabase()
    {
        // Arrange
        var product = new ProductBuilder().Build();
        await Context.Products.AddAsync(product);
        await Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(product.Id);
        await Context.SaveChangesAsync();

        // Assert
        var deletedProduct = await Context.Products.FindAsync(product.Id);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.CreateVersion7();

        // Act & Assert
        await _repository.DeleteAsync(nonExistentId);
        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task AddAsync_WithInvalidPrice_ShouldThrowException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new ProductBuilder().WithPrice(0).Build());
        Assert.Throws<ArgumentException>(() => new ProductBuilder().WithPrice(-1).Build());
    }
}