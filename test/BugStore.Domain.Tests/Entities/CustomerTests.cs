using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Constructor_ShouldCreateCustomerWithValidData()
    {
        // Arrange
        var name = "João Silva";
        var email = "joao@email.com";
        var phone = "(11) 99999-9999";
        var birthDate = new DateTime(1990, 5, 15);

        // Act
        var customer = new Customer(name, email, phone, birthDate);

        // Assert
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal(name, customer.Name);
        Assert.Equal(email, customer.Email);
        Assert.Equal(phone, customer.Phone);
        Assert.Equal(birthDate, customer.BirthDate);
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange
        var name = "Maria Santos";
        var email = "maria@email.com";
        var phone = "(11) 88888-8888";
        var birthDate = new DateTime(1985, 10, 20);

        // Act
        var customer1 = new Customer(name, email, phone, birthDate);
        var customer2 = new Customer(name, email, phone, birthDate);

        // Assert
        Assert.NotEqual(customer1.Id, customer2.Id);
    }

    [Fact]
    public void Update_ShouldUpdateAllProperties()
    {
        // Arrange
        var customer = new Customer("Nome Original", "original@email.com", "(11) 11111-1111", new DateTime(1980, 1, 1));
        var newName = "Nome Atualizado";
        var newEmail = "atualizado@email.com";
        var newPhone = "(11) 22222-2222";
        var newBirthDate = new DateTime(1985, 12, 31);

        // Act
        customer.Update(newName, newEmail, newPhone, newBirthDate);

        // Assert
        Assert.Equal(newName, customer.Name);
        Assert.Equal(newEmail, customer.Email);
        Assert.Equal(newPhone, customer.Phone);
        Assert.Equal(newBirthDate, customer.BirthDate);
    }
}