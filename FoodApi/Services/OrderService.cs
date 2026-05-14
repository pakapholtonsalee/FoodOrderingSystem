using FoodApi.Controllers;
using FoodApi.Hubs;
using FoodApi.Models;
using FoodApi.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace FoodApi.Services;

/// <summary>
/// Business logic สำหรับออเดอร์ (Service Layer Pattern)
/// แยก logic ออกจาก Controller และ Repository
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IHubContext<OrderHub> _hub;

    public OrderService(IOrderRepository repo, IHubContext<OrderHub> hub)
    {
        _repo = repo;
        _hub = hub;
    }

    public async Task<Order> PlaceOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            RestaurantId = request.RestaurantId,
            RestaurantName = request.RestaurantName,
            Status = request.Status ?? "Pending",
            CreatedAt = DateTime.UtcNow,
            Items = request.Items ?? new(),
            Total = request.Total
        };

        await _repo.CreateAsync(order);

        // แจ้ง Restaurant ผ่าน SignalR ทันทีหลังสร้างออเดอร์
        await _hub.Clients.All.SendAsync(
            "NewOrder",
            order.Id,
            order.CustomerName,
            order.RestaurantName,
            order.Items,
            order.Total,
            order.Status
        );

        return order;
    }

    public Task<List<Order>> GetAllOrdersAsync() => _repo.GetAllAsync();

    public Task<Order?> GetOrderByIdAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<Order?> ChangeOrderStatusAsync(int id, string newStatus)
    {
        var allowedStatuses = new[]
        {
        "Pending",
        "Preparing",
        "Completed",
        "Delivering",
        "Delivered"
    };

        if (!allowedStatuses.Contains(newStatus))
            return null;

        var order = await _repo.UpdateStatusAsync(id, newStatus);
        if (order == null) return null;

        await _hub.Clients.All.SendAsync("OrderStatusChanged", order.Id, order.Status);

        return order;
    }
}
