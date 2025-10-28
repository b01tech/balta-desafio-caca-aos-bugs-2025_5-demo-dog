using BugStore.Domain.Entities;

namespace BugStore.Infrastructure.Tests.Builders;

public class OrderBuilder
{
    private Guid _customerId = Guid.CreateVersion7();
    private Customer? _customer;
    private List<OrderLine> _lines = new();

    public OrderBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public OrderBuilder WithCustomer(Customer customer)
    {
        _customer = customer;
        _customerId = customer.Id;
        return this;
    }

    public OrderBuilder WithLines(List<OrderLine> lines)
    {
        _lines = lines;
        return this;
    }

    public OrderBuilder AddLine(OrderLine line)
    {
        _lines.Add(line);
        return this;
    }

    public Order Build()
    {
        var customer = _customer ?? new CustomerBuilder().Build();
        return new Order(_customerId, customer, _lines);
    }
}