using FoodApi.Models;
using FoodApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // POST /api/orders
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.PlaceOrderAsync(request);
        return Ok(OrderToDto(order));
    }

    // GET /api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders.Select(OrderToDto));
    }

    // GET /api/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return order == null ? NotFound() : Ok(OrderToDto(order));
    }

    // PATCH /api/orders/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var order = await _orderService.ChangeOrderStatusAsync(id, request.Status);
        return order == null ? NotFound() : Ok(OrderToDto(order));
    }

    private static object OrderToDto(Order o) => new
    {
        o.Id,
        CustomerId = o.CustomerId,
        CustomerName = o.Customer?.Username ?? "",
        RestaurantId = o.RestaurantId,
        RestaurantName = o.Restaurant?.Name ?? "",
        ChefId = o.ChefId,
        RiderId = o.RiderId,
        o.Status,
        o.TotalPrice,
        OrderDate = o.OrderDate,
        Items = o.Items.Select(i => new {
            i.FoodName,
            i.Quantity,
            i.Price
        }).ToList()
    };
}

// ── Request DTOs ──────────────────────────────────────────────

public class CreateOrderRequest
{
    public int? CustomerId { get; set; }   // FK → users.id
    public int? RestaurantId { get; set; }   // FK → restaurants.id
    public string? Status { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public string FoodName { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = "";
}