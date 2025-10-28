using BugStore.Application.Handlers.Customers;
using BugStore.Domain.Interfaces;
using BugStore.Domain.Entities;
using Moq;

namespace BugStore.Application.Tests.Handlers;

public class CustomerHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Handler _handler;

    public CustomerHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new Handler(_unitOfWorkMock.Object, _customerRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCustomer_WhenEmailDoesNotExist()
    {
        // Arrange
        var request = new BugStore.Application.DTOs.Requests.Customers.Create
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Phone = "(11) 99999-9999",
            BirthDate = new DateTime(1990, 5, 15)
        };

        _customerRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Customer>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Phone, result.Phone);
        Assert.Equal(request.BirthDate, result.BirthDate);

        _customerRepositoryMock.Verify(x => x.EmailExistsAsync(request.Email), Times.Once);
        _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new BugStore.Application.DTOs.Requests.Customers.Create
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Phone = "(11) 99999-9999",
            BirthDate = new DateTime(1990, 5, 15)
        };

        _customerRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.CreateAsync(request));
        Assert.Equal("Email já está em uso", exception.Message);

        _customerRepositoryMock.Verify(x => x.EmailExistsAsync(request.Email), Times.Once);
        _customerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var request = new BugStore.Application.DTOs.Requests.Customers.GetById { Id = customerId };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _handler.GetByIdAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(customer.Name, result.Name);
        Assert.Equal(customer.Email, result.Email);
        Assert.Equal(customer.Phone, result.Phone);
        Assert.Equal(customer.BirthDate, result.BirthDate);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Customers.GetById { Id = customerId };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.GetByIdAsync(request);

        // Assert
        Assert.Null(result);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15)),
            new Customer("Maria Santos", "maria@email.com", "(11) 88888-8888", new DateTime(1985, 10, 20)),
            new Customer("Pedro Costa", "pedro@email.com", "(11) 77777-7777", new DateTime(1992, 3, 8))
        };

        var request = new BugStore.Application.DTOs.Requests.Customers.Get
        {
            Page = 1,
            PageSize = 2
        };

        _customerRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await _handler.GetAllAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Page, result.Page);
        Assert.Equal(request.PageSize, result.PageSize);
        Assert.Equal(customers.Count, result.TotalCount);
        Assert.Equal(2, result.Customers.Count());

        _customerRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustomer_WhenCustomerExistsAndEmailIsValid()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var request = new BugStore.Application.DTOs.Requests.Customers.Update
        {
            Name = "João Santos",
            Email = "joao.santos@email.com",
            Phone = "(11) 88888-8888",
            BirthDate = new DateTime(1990, 5, 15)
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _customerRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(x => x.UpdateAsync(customer))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.UpdateAsync(customerId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Phone, result.Phone);
        Assert.Equal(request.BirthDate, result.BirthDate);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _customerRepositoryMock.Verify(x => x.EmailExistsAsync(request.Email), Times.Once);
        _customerRepositoryMock.Verify(x => x.UpdateAsync(customer), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Customers.Update
        {
            Name = "João Santos",
            Email = "joao.santos@email.com",
            Phone = "(11) 88888-8888",
            BirthDate = new DateTime(1990, 5, 15)
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.UpdateAsync(customerId, request);

        // Assert
        Assert.Null(result);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _customerRepositoryMock.Verify(x => x.EmailExistsAsync(It.IsAny<string>()), Times.Never);
        _customerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenEmailAlreadyExistsForAnotherCustomer()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var request = new BugStore.Application.DTOs.Requests.Customers.Update
        {
            Name = "João Santos",
            Email = "maria@email.com", // Email já existe para outro customer
            Phone = "(11) 88888-8888",
            BirthDate = new DateTime(1990, 5, 15)
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _customerRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.UpdateAsync(customerId, request));
        Assert.Equal("Email já está em uso", exception.Message);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _customerRepositoryMock.Verify(x => x.EmailExistsAsync(request.Email), Times.Once);
        _customerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCustomer_WhenCustomerExists()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var customer = new Customer("João Silva", "joao@email.com", "(11) 99999-9999", new DateTime(1990, 5, 15));
        var request = new BugStore.Application.DTOs.Requests.Customers.Delete { Id = customerId };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _customerRepositoryMock
            .Setup(x => x.DeleteAsync(customerId))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.DeleteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Customer excluído com sucesso", result.Message);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _customerRepositoryMock.Verify(x => x.DeleteAsync(customerId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var request = new BugStore.Application.DTOs.Requests.Customers.Delete { Id = customerId };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.DeleteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Customer não encontrado", result.Message);

        _customerRepositoryMock.Verify(x => x.GetByIdAsync(customerId), Times.Once);
        _customerRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}