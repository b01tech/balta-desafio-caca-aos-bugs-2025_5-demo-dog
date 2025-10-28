namespace BugStore.Application.DTOs.Requests.Orders;

public class Update
{
    public Guid Id { get; set; }
    public List<OrderLineRequest> Lines { get; set; } = new();
}