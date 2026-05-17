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

        // 🔔 Notification: ส่งให้ Chef ว่า "มีออเดอร์ใหม่"
        if (order.ChefId != null)
        {
            await _repo.AddNotificationAsync(
                order.ChefId.Value,
                order.Id,
                "new_order",
                $"มีออเดอร์ใหม่ #{order.Id} จากลูกค้า {order.Customer?.Username}",
                order.RestaurantId
            );
        }

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

        // 🔔 เพิ่ม Notification ตามสถานะ
        if (newStatus == "Preparing" && order.ChefId != null)
        {
            // Chef ได้รับการยืนยัน
            await _repo.AddNotificationAsync(
                order.ChefId.Value,
                order.Id,
                "order_confirmed",
                $"ออเดอร์ #{order.Id} ได้รับการยืนยัน - เริ่มจัดเตรียมอาหาร",
                order.RestaurantId
            );
            // Customer ได้รับแจ้ง
            if (order.CustomerId != null)
            {
                await _repo.AddNotificationAsync(
                    order.CustomerId.Value,
                    order.Id,
                    "order_preparing",
                    $"Chef กำลังจัดเตรียมอาหาร ออเดอร์ #{order.Id}",
                    order.RestaurantId
                );
            }
        }
        else if (newStatus == "Ready" && order.RiderId != null)
        {
            // Rider ได้รับแจ้ง
            await _repo.AddNotificationAsync(
                order.RiderId.Value,
                order.Id,
                "ready_for_pickup",
                $"อาหารเตรียมเสร็จ #{order.Id} - มาเบิกเลย",
                order.RestaurantId
            );
            // Customer ได้รับแจ้ง
            if (order.CustomerId != null)
            {
                await _repo.AddNotificationAsync(
                    order.CustomerId.Value,
                    order.Id,
                    "order_ready",
                    $"อาหารเตรียมเสร็จแล้ว #{order.Id} - รอการส่งมอบ",
                    order.RestaurantId
                );
            }
        }
        else if (newStatus == "PickedUp")
        {
            // Customer ได้รับแจ้ง
            if (order.CustomerId != null)
            {
                await _repo.AddNotificationAsync(
                    order.CustomerId.Value,
                    order.Id,
                    "on_the_way",
                    $"Rider กำลังมาส่ง ออเดอร์ #{order.Id}",
                    order.RestaurantId
                );
            }
        }
        else if (newStatus == "Delivered")
        {
            // Customer ได้รับแจ้ง
            if (order.CustomerId != null)
            {
                await _repo.AddNotificationAsync(
                    order.CustomerId.Value,
                    order.Id,
                    "delivered",
                    $"ลูกค้าได้รับอาหาร ออเดอร์ #{order.Id} สำเร็จ!",
                    order.RestaurantId
                );
            }
        }

        await _hub.Clients.All.SendAsync("OrderStatusChanged", order.Id, order.Status);
        return order;
    }
}