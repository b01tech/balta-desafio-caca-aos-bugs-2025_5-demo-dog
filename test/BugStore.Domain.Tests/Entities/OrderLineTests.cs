using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests.Entities;

public class OrderLineTests
{
    [Fact]
    public void Constructor_ShouldCreateOrderLineWithValidData()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var quantity = 3;
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500m);
        var productId = product.Id;
        var expectedTotal = quantity * product.Price;

        // Act
        var orderLine = new OrderLine(orderId, quantity, productId, product);

        // Assert
        Assert.NotEqual(Guid.Empty, orderLine.Id);
        Assert.Equal(orderId, orderLine.OrderId);
        Assert.Equal(quantity, orderLine.Quantity);
        Assert.Equal(productId, orderLine.ProductId);
        Assert.Equal(product, orderLine.Product);
        Assert.Equal(expectedTotal, orderLine.Total);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var quantity = 2;
        var product = new Product("Mouse Gamer", "Mouse Gamer RGB", "mouse-gamer", 150m);
        var productId = product.Id;

        // Act
        var orderLine1 = new OrderLine(orderId, quantity, productId, product);
        var orderLine2 = new OrderLine(orderId, quantity, productId, product);

        // Assert
        Assert.NotEqual(orderLine1.Id, orderLine2.Id);
    }

    [Fact]
    public void Constructor_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var quantity = 5;
        var product = new Product("Teclado Mecânico", "Teclado Mecânico RGB", "teclado-mecanico", 300.50m);
        var expectedTotal = quantity * product.Price; // 5 * 300.50 = 1502.50

        // Act
        var orderLine = new OrderLine(orderId, quantity, product.Id, product);

        // Assert
        Assert.Equal(expectedTotal, orderLine.Total);
        Assert.Equal(1502.50m, orderLine.Total);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenQuantityIsZero()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var quantity = 0;
        var product = new Product("Produto Teste", "Descrição Teste", "produto-teste", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new OrderLine(orderId, quantity, product.Id, product));
        Assert.Equal("Quantity must be greater than zero (Parameter 'quantity')", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenQuantityIsNegative()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var quantity = -5;
        var product = new Product("Produto Teste", "Descrição Teste", "produto-teste", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new OrderLine(orderId, quantity, product.Id, product));
        Assert.Equal("Quantity must be greater than zero (Parameter 'quantity')", exception.Message);
    }

    [Theory]
    [InlineData(1, 100.00, 100.00)]
    [InlineData(2, 50.50, 101.00)]
    [InlineData(10, 25.99, 259.90)]
    [InlineData(3, 333.33, 999.99)]
    public void Constructor_ShouldCalculateTotalCorrectly_WithDifferentValues(int quantity, decimal price, decimal expectedTotal)
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var product = new Product("Produto Teste", "Descrição Teste", "produto-teste", price);

        // Act
        var orderLine = new OrderLine(orderId, quantity, product.Id, product);

        // Assert
        Assert.Equal(expectedTotal, orderLine.Total);
    }
}