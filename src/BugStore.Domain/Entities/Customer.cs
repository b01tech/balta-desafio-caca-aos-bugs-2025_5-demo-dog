namespace BugStore.Domain.Entities;

public class Customer
{
    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public DateTime BirthDate { get; private set; }

    protected Customer() { }

    public Customer(string name, string email, string phone, DateTime birthDate)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
    }

    public void Update(string name, string email, string phone, DateTime birthDate)
    {
        Name = name;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
    }
}