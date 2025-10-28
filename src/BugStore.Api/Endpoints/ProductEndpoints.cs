using System.Text.Json;
using BugStore.Application.Handlers.Products;
using BugStore.Application.DTOs.Requests.Products;
using Microsoft.AspNetCore.Mvc;
using RequestDTOs = BugStore.Application.DTOs.Requests.Products;

namespace BugStore.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/v1/products").WithTags("Products");
            
            group.MapGet("/", GetAllProductsAsync).WithDescription("Get all products");
            group.MapGet("/{id}", GetProductByIdAsync).WithDescription("Get product by id");
            group.MapPost("/", CreateProductAsync).WithDescription("Create a new product");
            group.MapPut("/{id}", UpdateProductAsync).WithDescription("Update product by id");
            group.MapDelete("/{id}", DeleteProductAsync).WithDescription("Delete product by id");
        }

        private static async Task<IResult> GetAllProductsAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new RequestDTOs.Get { Page = page, PageSize = pageSize };
                var response = await handler.GetAllAsync(request);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Erro interno do servidor",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        private static async Task<IResult> GetProductByIdAsync(
            Guid id,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new RequestDTOs.GetById { Id = id };
                var response = await handler.GetByIdAsync(request);
                
                if (response == null)
                    return Results.NotFound(new { message = "Product não encontrado" });
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Erro interno do servidor",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        private static async Task<IResult> CreateProductAsync(
            HttpContext context,
            [FromServices] Handler handler = null!)
        {
            try
            {
                // Ler o JSON manualmente
                using var reader = new StreamReader(context.Request.Body);
                var json = await reader.ReadToEndAsync();
                Console.WriteLine($"Raw JSON received: {json}");
                
                var request = JsonSerializer.Deserialize<RequestDTOs.Create>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                Console.WriteLine($"Deserialized request: Title={request?.Title}, Description={request?.Description}, Slug={request?.Slug}, Price={request?.Price}");
                
                if (request == null)
                {
                    return Results.BadRequest(new { message = "Request body é obrigatório" });
                }

                var response = await handler.CreateAsync(request);
                return Results.Created($"/v1/products/{response.Id}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateProductAsync: {ex}");
                return Results.Problem(
                    title: "Erro interno do servidor",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        private static async Task<IResult> UpdateProductAsync(
            Guid id,
            HttpContext context,
            [FromServices] Handler handler = null!)
        {
            try
            {
                // Ler o JSON manualmente
                using var reader = new StreamReader(context.Request.Body);
                var json = await reader.ReadToEndAsync();
                Console.WriteLine($"Raw JSON received for update: {json}");
                
                var request = JsonSerializer.Deserialize<RequestDTOs.Update>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                Console.WriteLine($"Deserialized update request: Title={request?.Title}, Description={request?.Description}, Slug={request?.Slug}, Price={request?.Price}");
                
                if (request == null)
                {
                    return Results.BadRequest(new { message = "Request body é obrigatório" });
                }

                var response = await handler.UpdateAsync(id, request);
                
                if (response == null)
                    return Results.NotFound(new { message = "Product não encontrado" });
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Erro interno do servidor",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        private static async Task<IResult> DeleteProductAsync(
            Guid id,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new RequestDTOs.Delete { Id = id };
                var response = await handler.DeleteAsync(request);
                
                if (response.Message.Contains("não encontrado"))
                    return Results.NotFound(new { message = response.Message });
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Erro interno do servidor",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }
    }
}