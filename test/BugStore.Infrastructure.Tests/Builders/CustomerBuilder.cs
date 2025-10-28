using BugStore.Domain.Entities;

namespace BugStore.Infrastructure.Tests.Builders;

public class CustomerBuilder
{
    private string _name = "Test Customer";
    private string _email = "test@example.com";
    private string _phone = "123456789";
    private DateTime _birthDate = new DateTime(1990, 1, 1);

    public CustomerBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CustomerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CustomerBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    public CustomerBuilder WithBirthDate(DateTime birthDate)
    {
        _birthDate = birthDate;
        return this;
    }

    public Customer Build()
    {
        return new Customer(_name, _email, _phone, _birthDate);
    }
}