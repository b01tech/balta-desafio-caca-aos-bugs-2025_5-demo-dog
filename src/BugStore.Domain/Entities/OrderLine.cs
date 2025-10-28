namespace BugStore.Domain.Entities;

public class OrderLine
{
    public Guid Id { get; init; }
    public Guid OrderId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Total { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    protected OrderLine() { }

    public OrderLine(Guid orderId, int quantity, Guid productId, Product product)
    {
        ValidateQuantity(quantity);
        Id = Guid.CreateVersion7();
        OrderId = orderId;
        Quantity = quantity;
        Total = quantity * product.Price;
        ProductId = productId;
        Product = product;
    }
    private void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
    }
}