namespace BugStore.Application.DTOs.Responses.Orders;

public class Create
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderLineResponse> Lines { get; set; } = new();
}

public class OrderLineResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Total { get; set; }
}
