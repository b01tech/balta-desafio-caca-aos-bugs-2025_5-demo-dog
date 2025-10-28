namespace BugStore.Application.DTOs.Responses.Customers;

public class Get
{
    public IEnumerable<GetById> Customers { get; set; } = new List<GetById>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
