using FoodApi.Data;
using FoodApi.Hubs;
using FoodApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FoodApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly FoodContext _context;

    private readonly IHubContext<OrderHub> _hub;

    public OrdersController(
        FoodContext context,
        IHubContext<OrderHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        await _hub.Clients.All.SendAsync(
            "NewOrder",
            order.Id,
            order.CustomerName,
            order.Items,
            order.Total);
        return Ok(order);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Orders.ToList());
    }
}