using System.Text.Json;
using BugStore.Application.Handlers.Customers;
using BugStore.Application.DTOs.Requests.Customers;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void MapCustomerEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/v1/customers").WithTags("Customers");
            
            group.MapGet("/", GetAllCustomersAsync)
                .WithName("GetAllCustomers")
                .WithDescription("Get all customers with pagination");
            
            group.MapGet("/{id:guid}", GetCustomerByIdAsync)
                .WithName("GetCustomerById")
                .WithDescription("Get customer by id");
            
            group.MapPost("/", CreateCustomerAsync)
                .WithName("CreateCustomer")
                .WithDescription("Create a new customer");
            
            group.MapPut("/{id:guid}", UpdateCustomerAsync)
                .WithName("UpdateCustomer")
                .WithDescription("Update customer by id");
            
            group.MapDelete("/{id:guid}", DeleteCustomerAsync)
                .WithName("DeleteCustomer")
                .WithDescription("Delete customer by id");
        }

        private static async Task<IResult> GetAllCustomersAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new Get { Page = page, PageSize = pageSize };
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

        private static async Task<IResult> GetCustomerByIdAsync(
            Guid id,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new GetById { Id = id };
                var response = await handler.GetByIdAsync(request);
                
                if (response == null)
                    return Results.NotFound(new { message = "Customer não encontrado" });
                
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

        private static async Task<IResult> CreateCustomerAsync(
            HttpContext context,
            [FromServices] Handler handler = null!)
        {
            try
            {
                // Ler o JSON manualmente
                using var reader = new StreamReader(context.Request.Body);
                var json = await reader.ReadToEndAsync();
                Console.WriteLine($"Raw JSON received: {json}");
                
                var request = JsonSerializer.Deserialize<Create>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                Console.WriteLine($"Deserialized request: Name={request?.Name}, Email={request?.Email}, Phone={request?.Phone}, BirthDate={request?.BirthDate}");
                
                if (request == null)
                {
                    return Results.BadRequest(new { message = "Request body é obrigatório" });
                }

                var response = await handler.CreateAsync(request);
                return Results.Created($"/v1/customers/{response.Id}", response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateCustomerAsync: {ex}");
                return Results.Problem(
                    title: "Erro interno do servidor",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        private static async Task<IResult> UpdateCustomerAsync(
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
                
                var request = JsonSerializer.Deserialize<Update>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                Console.WriteLine($"Deserialized update request: Name={request?.Name}, Email={request?.Email}, Phone={request?.Phone}, BirthDate={request?.BirthDate}");
                
                if (request == null)
                {
                    return Results.BadRequest(new { message = "Request body é obrigatório" });
                }

                var response = await handler.UpdateAsync(id, request);
                
                if (response == null)
                    return Results.NotFound(new { message = "Customer não encontrado" });
                
                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateCustomerAsync: {ex}");
                return Results.Problem(
                    title: "Erro interno do servidor",
                    detail: ex.Message,
                    statusCode: 500
                );
            }
        }

        private static async Task<IResult> DeleteCustomerAsync(
            Guid id,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new Delete { Id = id };
                var response = await handler.DeleteAsync(request);
                
                if (!response.Success)
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