namespace BugStore.Application.DTOs.Responses.Orders;

public class Get
{
    public List<GetById> Orders { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}