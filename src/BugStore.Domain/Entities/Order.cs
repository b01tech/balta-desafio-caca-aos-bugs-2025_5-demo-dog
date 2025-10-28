namespace BugStore.Domain.Entities;

public class Order
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public List<OrderLine> Lines { get; private set; } = new();

    protected Order() { }

    public Order(Guid customerId, Customer customer, List<OrderLine> lines)
    {
        Id = Guid.CreateVersion7();
        CustomerId = customerId;
        Customer = customer;
        Lines = lines;
    }

    public void UpdateLines(List<OrderLine> lines)
    {
        Lines.Clear();
        Lines.AddRange(lines);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AddLine(OrderLine line)
    {
        Lines.Add(line);
        UpdatedAt = DateTime.UtcNow;
    }
    public void RemoveLine(OrderLine line)
    {
        Lines.Remove(line);
        UpdatedAt = DateTime.UtcNow;
    }

}