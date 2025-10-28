namespace BugStore.Application.DTOs.Requests.Products;

public class Get
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
