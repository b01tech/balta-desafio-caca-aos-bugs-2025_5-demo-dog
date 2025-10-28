using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldCreateProductWithValidData()
    {
        // Arrange
        var title = "Notebook Dell";
        var description = "Notebook Dell Inspiron 15 3000";
        var slug = "notebook-dell-inspiron-15";
        var price = 2500.99m;

        // Act
        var product = new Product(title, description, slug, price);

        // Assert
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal(title, product.Title);
        Assert.Equal(description, product.Description);
        Assert.Equal(slug, product.Slug);
        Assert.Equal(price, product.Price);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange
        var title = "Mouse Gamer";
        var description = "Mouse Gamer RGB";
        var slug = "mouse-gamer-rgb";
        var price = 150.00m;

        // Act
        var product1 = new Product(title, description, slug, price);
        var product2 = new Product(title, description, slug, price);

        // Assert
        Assert.NotEqual(product1.Id, product2.Id);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsZero()
    {
        // Arrange
        var title = "Produto Inválido";
        var description = "Produto com preço zero";
        var slug = "produto-invalido";
        var price = 0m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Product(title, description, slug, price));
        Assert.Equal("Price must be greater than zero (Parameter 'price')", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenPriceIsNegative()
    {
        // Arrange
        var title = "Produto Inválido";
        var description = "Produto com preço negativo";
        var slug = "produto-invalido";
        var price = -100m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Product(title, description, slug, price));
        Assert.Equal("Price must be greater than zero (Parameter 'price')", exception.Message);
    }

    [Fact]
    public void Update_ShouldUpdateAllProperties_WhenPriceIsValid()
    {
        // Arrange
        var product = new Product("Título Original", "Descrição Original", "slug-original", 100m);
        var newTitle = "Título Atualizado";
        var newDescription = "Descrição Atualizada";
        var newSlug = "slug-atualizado";
        var newPrice = 200m;

        // Act
        product.Update(newTitle, newDescription, newSlug, newPrice);

        // Assert
        Assert.Equal(newTitle, product.Title);
        Assert.Equal(newDescription, product.Description);
        Assert.Equal(newSlug, product.Slug);
        Assert.Equal(newPrice, product.Price);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenPriceIsInvalid()
    {
        // Arrange
        var product = new Product("Título Original", "Descrição Original", "slug-original", 100m);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            product.Update("Novo Título", "Nova Descrição", "novo-slug", -50m));
        Assert.Equal("Price must be greater than zero (Parameter 'price')", exception.Message);
    }
}