namespace BugStore.Application.DTOs.Responses.Orders;

public class Update
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderLineResponse> Lines { get; set; } = new();
    public decimal TotalAmount => Lines.Sum(l => l.Total);
}