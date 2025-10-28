namespace BugStore.Domain.Entities;

public class Product
{
    public Guid Id { get; init; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    protected Product() { }

    public Product(string title, string description, string slug, decimal price)
    {
        ValidatePrice(price);
        Id = Guid.CreateVersion7();
        Title = title;
        Description = description;
        Slug = slug;
        Price = price;
    }

    private void ValidatePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));
    }
    public void Update(string title, string description, string slug, decimal price)
    {
        ValidatePrice(price);
        Title = title;
        Description = description;
        Slug = slug;
        Price = price;
    }
}