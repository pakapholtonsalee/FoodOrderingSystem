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

    // POST /api/orders — สร้างออเดอร์ใหม่
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.PlaceOrderAsync(request);
        return Ok(OrderToDto(order));
    }

    // GET /api/orders — ดูออเดอร์ทั้งหมด (เรียงล่าสุดก่อน)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders.Select(OrderToDto));
    }

    // GET /api/orders/{id} — ดูออเดอร์ตาม ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return order == null ? NotFound() : Ok(OrderToDto(order));
    }

    // PATCH /api/orders/{id}/status — อัปเดตสถานะออเดอร์
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var order = await _orderService.ChangeOrderStatusAsync(id, request.Status);
        return order == null ? NotFound() : Ok(OrderToDto(order));
    }

    // แปลง Order → DTO ที่ส่งให้ Client (ส่ง Items เป็น List<string> เลย)
    private static object OrderToDto(Order o) => new
    {
        o.Id,
        o.CustomerName,
        o.RestaurantId,
        o.Status,
        o.CreatedAt,
        o.Items,
        o.Total
    };
}

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = "";
    public int RestaurantId { get; set; }
    public string? Status { get; set; }
    public List<string> Items { get; set; } = new();
    public int Total { get; set; }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = "";
}