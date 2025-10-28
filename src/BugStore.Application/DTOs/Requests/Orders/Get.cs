namespace BugStore.Application.DTOs.Requests.Orders;

public class Get
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? CustomerId { get; set; }
}