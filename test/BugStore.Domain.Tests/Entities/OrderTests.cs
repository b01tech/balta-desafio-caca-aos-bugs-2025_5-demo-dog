using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldCreateOrderWithValidData()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var lines = new List<OrderLine>();

        // Act
        var order = new Order(customerId, customer, lines);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(customer, order.Customer);
        Assert.NotEqual(default(DateTime), order.CreatedAt);
        Assert.Null(order.UpdatedAt);
        Assert.Empty(order.Lines);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("Maria Santos", "maria@email.com", "(11) 88888-8888", new DateTime(1985, 10, 20));
        var lines = new List<OrderLine>();

        // Act
        var order1 = new Order(customerId, customer, lines);
        var order2 = new Order(customerId, customer, lines);

        // Assert
        Assert.NotEqual(order1.Id, order2.Id);
    }

    [Fact]
    public void AddLine_ShouldAddOrderLineAndSetUpdatedAt()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var order = new Order(customerId, customer, new List<OrderLine>());

        var product = new Product("Notebook", "Notebook Dell", "notebook-dell", 2500m);
        var orderLine = new OrderLine(order.Id, 2, product.Id, product);

        // Act
        order.AddLine(orderLine);

        // Assert
        Assert.Single(order.Lines);
        Assert.Contains(orderLine, order.Lines);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void RemoveLine_ShouldRemoveOrderLineAndSetUpdatedAt()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var product = new Product("Notebook", "Notebook Dell", "notebook-dell", 2500m);
        var orderLine = new OrderLine(Guid.CreateVersion7(), 2, product.Id, product);
        var lines = new List<OrderLine> { orderLine };
        var order = new Order(customerId, customer, lines);

        // Act
        order.RemoveLine(orderLine);

        // Assert
        Assert.Empty(order.Lines);
        Assert.DoesNotContain(orderLine, order.Lines);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void UpdateLines_ShouldReplaceAllLinesAndSetUpdatedAt()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var product1 = new Product("Notebook", "Notebook Dell", "notebook-dell", 2500m);
        var product2 = new Product("Mouse", "Mouse Gamer", "mouse-gamer", 150m);

        var oldLine = new OrderLine(Guid.CreateVersion7(), 1, product1.Id, product1);
        var order = new Order(customerId, customer, new List<OrderLine> { oldLine });

        var newLines = new List<OrderLine>
        {
            new OrderLine(order.Id, 2, product2.Id, product2)
        };

        // Act
        order.UpdateLines(newLines);

        // Assert
        Assert.Single(order.Lines);
        Assert.DoesNotContain(oldLine, order.Lines);
        Assert.Contains(newLines[0], order.Lines);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void SetUpdatedAt_ShouldSetUpdatedAtProperty()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var order = new Order(customerId, customer, new List<OrderLine>());
        var initialUpdatedAt = order.UpdatedAt;

        // Act
        order.SetUpdatedAt();

        // Assert
        Assert.NotEqual(initialUpdatedAt, order.UpdatedAt);
        Assert.NotNull(order.UpdatedAt);
    }
}