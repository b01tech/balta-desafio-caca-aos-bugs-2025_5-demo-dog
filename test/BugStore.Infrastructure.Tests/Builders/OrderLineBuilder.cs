using BugStore.Domain.Entities;

namespace BugStore.Infrastructure.Tests.Builders;

public class OrderLineBuilder
{
    private Guid _orderId = Guid.CreateVersion7();
    private int _quantity = 1;
    private Guid _productId = Guid.CreateVersion7();
    private Product? _product;

    public OrderLineBuilder WithOrderId(Guid orderId)
    {
        _orderId = orderId;
        return this;
    }

    public OrderLineBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public OrderLineBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public OrderLineBuilder WithProduct(Product product)
    {
        _product = product;
        _productId = product.Id;
        return this;
    }

    public OrderLine Build()
    {
        var product = _product ?? new ProductBuilder().Build();
        return new OrderLine(_orderId, _quantity, _productId, product);
    }
}