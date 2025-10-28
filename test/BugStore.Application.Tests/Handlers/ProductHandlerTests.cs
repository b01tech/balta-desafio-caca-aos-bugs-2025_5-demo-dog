using BugStore.Application.Handlers.Products;
using BugStore.Domain.Interfaces;
using BugStore.Domain.Entities;
using Moq;

namespace BugStore.Application.Tests.Handlers;

public class ProductHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Handler _handler;

    public ProductHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new Handler(_unitOfWorkMock.Object, _productRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WithValidData()
    {
        // Arrange
        var request = new BugStore.Application.DTOs.Requests.Products.Create
        {
            Title = "Notebook Dell",
            Description = "Notebook Dell Inspiron 15",
            Slug = "notebook-dell",
            Price = 2500.00m
        };

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Slug, result.Slug);
        Assert.Equal(request.Price, result.Price);

        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m);
        var request = new BugStore.Application.DTOs.Requests.Products.GetById { Id = productId };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.GetByIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Title, result.Title);
        Assert.Equal(product.Description, result.Description);
        Assert.Equal(product.Slug, result.Slug);
        Assert.Equal(product.Price, result.Price);

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Products.GetById { Id = productId };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.GetByIdAsync(request);

        // Assert
        Assert.Null(result);

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m),
            new Product("Mouse Gamer", "Mouse Gamer RGB", "mouse-gamer", 150.00m),
            new Product("Teclado Mecânico", "Teclado Mecânico RGB", "teclado-mecanico", 300.00m)
        };

        var request = new BugStore.Application.DTOs.Requests.Products.Get
        {
            Page = 1,
            PageSize = 2
        };

        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _handler.GetAllAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Page, result.Page);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(products.Count, result.TotalCount);
        Assert.Equal(2, result.Products.Count());

        _productRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m);
        var request = new BugStore.Application.DTOs.Requests.Products.Update
        {
            Title = "Notebook Dell Atualizado",
            Description = "Notebook Dell Inspiron 15 Atualizado",
            Slug = "notebook-dell-atualizado",
            Price = 2800.00m
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(product))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.UpdateAsync(productId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Slug, result.Slug);
        Assert.Equal(request.Price, result.Price);

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(product), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Products.Update
        {
            Title = "Notebook Dell Atualizado",
            Description = "Notebook Dell Inspiron 15 Atualizado",
            Slug = "notebook-dell-atualizado",
            Price = 2800.00m
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.UpdateAsync(productId, request);

        // Assert
        Assert.Null(result);

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var product = new Product("Notebook Dell", "Notebook Dell Inspiron 15", "notebook-dell", 2500.00m);
        var request = new BugStore.Application.DTOs.Requests.Products.Delete { Id = productId };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.DeleteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Product excluído com sucesso", result.Message);

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFoundMessage_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Products.Delete { Id = productId };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.DeleteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Product não encontrado", result.Message);

        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}