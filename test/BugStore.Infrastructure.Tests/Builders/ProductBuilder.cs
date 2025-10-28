using BugStore.Domain.Entities;

namespace BugStore.Infrastructure.Tests.Builders;

public class ProductBuilder
{
    private string _title = "Test Product";
    private string _description = "Test Description";
    private string _slug = "test-product";
    private decimal _price = 10.99m;

    public ProductBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ProductBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProductBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public Product Build()
    {
        return new Product(_title, _description, _slug, _price);
    }
}