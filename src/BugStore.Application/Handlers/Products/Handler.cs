using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;

namespace BugStore.Application.Handlers.Products;

public class Handler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;

    public Handler(IUnitOfWork unitOfWork, IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
    }

    public async Task<DTOs.Responses.Products.Create> CreateAsync(DTOs.Requests.Products.Create request)
    {
        var product = new Product(request.Title, request.Description, request.Slug, request.Price);
        
        await _productRepository.AddAsync(product);
        await _unitOfWork.CommitAsync();

        return new DTOs.Responses.Products.Create
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Slug = product.Slug,
            Price = product.Price
        };
    }

    public async Task<DTOs.Responses.Products.GetById?> GetByIdAsync(DTOs.Requests.Products.GetById request)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        
        if (product == null)
            return null;

        return new DTOs.Responses.Products.GetById
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Slug = product.Slug,
            Price = product.Price
        };
    }

    public async Task<DTOs.Responses.Products.Get> GetAllAsync(DTOs.Requests.Products.Get request)
    {
        var products = await _productRepository.GetAllAsync();
        var productsList = products.ToList();
        
        var pagedProducts = productsList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new DTOs.Responses.Products.GetById
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Slug = p.Slug,
                Price = p.Price
            });

        return new DTOs.Responses.Products.Get
        {
            Products = pagedProducts,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = productsList.Count
        };
    }

    public async Task<DTOs.Responses.Products.Update?> UpdateAsync(Guid id, DTOs.Requests.Products.Update request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return null;

        product.Update(request.Title, request.Description, request.Slug, request.Price);
        
        await _productRepository.UpdateAsync(product);
        await _unitOfWork.CommitAsync();

        return new DTOs.Responses.Products.Update
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Slug = product.Slug,
            Price = product.Price
        };
    }

    public async Task<DTOs.Responses.Products.Delete> DeleteAsync(DTOs.Requests.Products.Delete request)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        
        if (product == null)
        {
            return new DTOs.Responses.Products.Delete
            {
                Message = "Product não encontrado"
            };
        }

        await _productRepository.DeleteAsync(request.Id);
        await _unitOfWork.CommitAsync();

        return new DTOs.Responses.Products.Delete
        {
            Message = "Product excluído com sucesso"
        };
    }
}