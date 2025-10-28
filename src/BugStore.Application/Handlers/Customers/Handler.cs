using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;

namespace BugStore.Application.Handlers.Customers;

public class Handler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;

    public Handler(IUnitOfWork unitOfWork, ICustomerRepository customerRepository)
    {
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
    }

    public async Task<DTOs.Responses.Customers.Create> CreateAsync(DTOs.Requests.Customers.Create request)
    {
        // Verificar se email já existe
        if (await _customerRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Email já está em uso");
        }

        var customer = new Customer(request.Name, request.Email, request.Phone, request.BirthDate);
        
        await _customerRepository.AddAsync(customer);
        await _unitOfWork.CommitAsync();

        return new DTOs.Responses.Customers.Create
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            BirthDate = customer.BirthDate
        };
    }

    public async Task<DTOs.Responses.Customers.GetById?> GetByIdAsync(DTOs.Requests.Customers.GetById request)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        
        if (customer == null)
            return null;

        return new DTOs.Responses.Customers.GetById
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            BirthDate = customer.BirthDate
        };
    }

    public async Task<DTOs.Responses.Customers.Get> GetAllAsync(DTOs.Requests.Customers.Get request)
    {
        var customers = await _customerRepository.GetAllAsync();
        var customersList = customers.ToList();
        
        var pagedCustomers = customersList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new DTOs.Responses.Customers.GetById
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                BirthDate = c.BirthDate
            });

        return new DTOs.Responses.Customers.Get
        {
            Customers = pagedCustomers,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = customersList.Count
        };
    }

    public async Task<DTOs.Responses.Customers.Update?> UpdateAsync(Guid id, DTOs.Requests.Customers.Update request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        
        if (customer == null)
            return null;

        // Verificar se email já existe para outro customer
        if (await _customerRepository.EmailExistsAsync(request.Email) && customer.Email != request.Email)
        {
            throw new InvalidOperationException("Email já está em uso");
        }

        customer.Update(request.Name, request.Email, request.Phone, request.BirthDate);
        
        await _customerRepository.UpdateAsync(customer);
        await _unitOfWork.CommitAsync();

        return new DTOs.Responses.Customers.Update
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            BirthDate = customer.BirthDate
        };
    }

    public async Task<DTOs.Responses.Customers.Delete> DeleteAsync(DTOs.Requests.Customers.Delete request)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        
        if (customer == null)
        {
            return new DTOs.Responses.Customers.Delete
            {
                Success = false,
                Message = "Customer não encontrado"
            };
        }

        await _customerRepository.DeleteAsync(request.Id);
        await _unitOfWork.CommitAsync();

        return new DTOs.Responses.Customers.Delete
        {
            Success = true,
            Message = "Customer excluído com sucesso"
        };
    }
}
