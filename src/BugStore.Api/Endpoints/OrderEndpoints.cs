
using System.Text.Json;
using BugStore.Application.Handlers.Orders;
using BugStore.Application.DTOs.Requests.Orders;
using Microsoft.AspNetCore.Mvc;
using RequestDTOs = BugStore.Application.DTOs.Requests.Orders;

namespace BugStore.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/v1/orders").WithTags("Orders");
            
            group.MapGet("/", GetAllOrdersAsync).WithDescription("Get all orders");
            group.MapGet("/{id}", GetOrderByIdAsync).WithDescription("Get order by id");
            group.MapPost("/", CreateOrderAsync).WithDescription("Create a new order");
            group.MapPut("/{id}", UpdateOrderAsync).WithDescription("Update an order");
            group.MapDelete("/{id}", DeleteOrderAsync).WithDescription("Delete an order");
        }

        private static async Task<IResult> GetAllOrdersAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? customerId = null,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new RequestDTOs.Get
                {
                    Page = page,
                    PageSize = pageSize,
                    CustomerId = customerId
                };

                var response = await handler.GetAllAsync(request);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro interno do servidor: {ex.Message}");
            }
        }

        private static async Task<IResult> GetOrderByIdAsync(
            Guid id,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new RequestDTOs.GetById { Id = id };
                var response = await handler.GetByIdAsync(request);
                
                if (response == null)
                    return Results.NotFound(new { message = "Order não encontrado" });
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro interno do servidor: {ex.Message}");
            }
        }

        private static async Task<IResult> CreateOrderAsync(
            HttpContext context,
            [FromServices] Handler handler = null!)
        {
            try
            {
                // Ler o JSON manualmente
                using var reader = new StreamReader(context.Request.Body);
                var json = await reader.ReadToEndAsync();
                
                var request = JsonSerializer.Deserialize<RequestDTOs.Create>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                if (request == null)
                {
                    return Results.BadRequest(new { message = "Request body é obrigatório" });
                }

                var response = await handler.CreateAsync(request);
                
                if (response == null)
                    return Results.BadRequest(new { message = "Não foi possível criar o order. Verifique se o customer e produtos existem." });
                
                return Results.Created($"/v1/orders/{response.Id}", response);
            }
            catch (JsonException)
            {
                return Results.BadRequest(new { message = "JSON inválido" });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro interno do servidor: {ex.Message}");
            }
        }

        private static async Task<IResult> UpdateOrderAsync(
            Guid id,
            HttpContext context,
            [FromServices] Handler handler = null!)
        {
            try
            {
                // Ler o JSON manualmente
                using var reader = new StreamReader(context.Request.Body);
                var json = await reader.ReadToEndAsync();
                
                var requestData = JsonSerializer.Deserialize<RequestDTOs.Update>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                if (requestData == null)
                {
                    return Results.BadRequest(new { message = "Request body é obrigatório" });
                }

                // Definir o ID do request
                requestData.Id = id;

                var response = await handler.UpdateAsync(requestData);
                
                if (response == null)
                    return Results.NotFound(new { message = "Order não encontrado ou produtos inválidos" });
                
                return Results.Ok(response);
            }
            catch (JsonException)
            {
                return Results.BadRequest(new { message = "JSON inválido" });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro interno do servidor: {ex.Message}");
            }
        }

        private static async Task<IResult> DeleteOrderAsync(
            Guid id,
            [FromServices] Handler handler = null!)
        {
            try
            {
                var request = new RequestDTOs.Delete { Id = id };
                var response = await handler.DeleteAsync(request);
                
                if (response == null)
                    return Results.NotFound(new { message = "Order não encontrado" });
                
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}