namespace BugStore.Application.DTOs.Responses.Products;

public class Get
{
    public IEnumerable<GetById> Products { get; set; } = new List<GetById>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
