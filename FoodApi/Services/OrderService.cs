using FoodApi.Controllers;
using FoodApi.Hubs;
using FoodApi.Models;
using FoodApi.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace FoodApi.Services;

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
            CustomerId = request.CustomerId,
            RestaurantId = request.RestaurantId,
            Status = request.Status ?? "Pending",
            OrderDate = DateTime.UtcNow,
            Items = request.Items.Select(i => new OrderItem
            {
                FoodName = i.FoodName,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList(),
            TotalPrice = request.Items.Sum(i => i.Price * i.Quantity)
        };

        await _repo.CreateAsync(order);

        await _hub.Clients.All.SendAsync(
            "NewOrder",
            order.Id,
            order.Customer?.Username ?? "",
            order.Restaurant?.Name ?? "",
            order.Items.Select(i => i.FoodName).ToList(),
            (int)order.TotalPrice,
            order.Status
        );

        return order;
    }

    public Task<List<Order>> GetAllOrdersAsync() => _repo.GetAllAsync();
    public Task<Order?> GetOrderByIdAsync(int id) => _repo.GetByIdAsync(id);

    public async Task<Order?> ChangeOrderStatusAsync(int id, string newStatus)
    {
        // ✅ ตรงกับ CHECK constraint ใน table
        var allowed = new[] { "Pending", "Confirmed", "Preparing", "Ready", "PickedUp", "Delivered", "Cancelled" };
        if (!allowed.Contains(newStatus)) return null;

        var order = await _repo.UpdateStatusAsync(id, newStatus);
        if (order == null) return null;

        await _hub.Clients.All.SendAsync("OrderStatusChanged", order.Id, order.Status);
        return order;
    }
}