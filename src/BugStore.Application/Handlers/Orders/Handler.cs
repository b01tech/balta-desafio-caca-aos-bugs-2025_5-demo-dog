using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Application.DTOs.Responses.Orders;

namespace BugStore.Application.Handlers.Orders;

public class Handler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public Handler(
        IUnitOfWork unitOfWork, 
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<DTOs.Responses.Orders.Create?> CreateAsync(DTOs.Requests.Orders.Create request)
    {
        try
        {
            // Verificar se o customer existe
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null)
                return null;

            // Criar o pedido primeiro
        var order = new Order(request.CustomerId, customer, new List<OrderLine>());
        
        // Criar as linhas do pedido
        foreach (var lineRequest in request.Lines)
        {
            var product = await _productRepository.GetByIdAsync(lineRequest.ProductId);
            if (product == null)
                throw new ArgumentException($"Product with ID {lineRequest.ProductId} not found");

            var line = new OrderLine(order.Id, lineRequest.Quantity, lineRequest.ProductId, product);
            order.AddLine(line);
        }

        // Adicionar o pedido ao repositório (isso vai adicionar as linhas também)
        await _orderRepository.AddAsync(order);
        await _unitOfWork.CommitAsync();

            return new DTOs.Responses.Orders.Create
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Lines = order.Lines.Select(l => new OrderLineResponse
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    ProductTitle = l.Product.Title,
                    Quantity = l.Quantity,
                    Total = l.Total
                }).ToList()
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<DTOs.Responses.Orders.GetById?> GetByIdAsync(DTOs.Requests.Orders.GetById request)
    {
        var order = await _orderRepository.GetWithOrderLinesAsync(request.Id);
        
        if (order == null)
            return null;

        return new DTOs.Responses.Orders.GetById
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.Name ?? string.Empty,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Lines = order.Lines?.Select(l => new OrderLineResponse
            {
                Id = l.Id,
                ProductId = l.ProductId,
                ProductTitle = l.Product?.Title ?? string.Empty,
                Quantity = l.Quantity,
                Total = l.Total
            }).ToList() ?? new List<OrderLineResponse>()
        };
    }

    public async Task<DTOs.Responses.Orders.Get> GetAllAsync(DTOs.Requests.Orders.Get request)
    {
        IEnumerable<Order> orders;
        
        if (request.CustomerId.HasValue)
        {
            orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId.Value);
        }
        else
        {
            orders = await _orderRepository.GetAllAsync(request.Page, request.PageSize);
        }

        var ordersList = orders.ToList();
        
        return new DTOs.Responses.Orders.Get
        {
            Orders = ordersList.Select(o => new DTOs.Responses.Orders.GetById
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name ?? string.Empty,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                Lines = o.Lines?.Select(l => new OrderLineResponse
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    ProductTitle = l.Product?.Title ?? string.Empty,
                    Quantity = l.Quantity,
                    Total = l.Total
                }).ToList() ?? new List<OrderLineResponse>()
            }).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = ordersList.Count
        };
    }

    public async Task<DTOs.Responses.Orders.Update?> UpdateAsync(DTOs.Requests.Orders.Update request)
    {
        try
        {
            var order = await _orderRepository.GetWithOrderLinesAsync(request.Id);
            if (order == null)
                return null;

            var newOrderLines = new List<OrderLine>();
            foreach (var line in request.Lines)
            {
                var product = await _productRepository.GetByIdAsync(line.ProductId);
                if (product == null)
                    return null;

                newOrderLines.Add(new OrderLine(order.Id, line.Quantity, line.ProductId, product));
            }

            // Usar o método UpdateLines da entidade Order para atualizar as linhas
            order.UpdateLines(newOrderLines);

            // Atualizar o pedido através do repositório
            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            // Recarregar o pedido com as linhas atualizadas
            var updatedOrder = await _orderRepository.GetWithOrderLinesAsync(order.Id);
            
            return new DTOs.Responses.Orders.Update
            {
                Id = updatedOrder.Id,
                CustomerId = updatedOrder.CustomerId,
                CreatedAt = updatedOrder.CreatedAt,
                UpdatedAt = updatedOrder.UpdatedAt,
                Lines = updatedOrder.Lines.Select(l => new OrderLineResponse
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    ProductTitle = l.Product.Title,
                    Quantity = l.Quantity,
                    Total = l.Total
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<DTOs.Responses.Orders.Delete?> DeleteAsync(DTOs.Requests.Orders.Delete request)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null)
                return null;

            await _orderRepository.DeleteAsync(request.Id);
            await _unitOfWork.CommitAsync();

            return new DTOs.Responses.Orders.Delete
            {
                Message = "Order excluído com sucesso"
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}