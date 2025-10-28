namespace BugStore.Application.DTOs.Requests.Orders;

public class Create
{
    public Guid CustomerId { get; set; }
    public List<OrderLineRequest> Lines { get; set; } = new();
}

public class OrderLineRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
