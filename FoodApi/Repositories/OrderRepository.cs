using FoodApi.Data;
using FoodApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly FoodContext _context;

    public OrderRepository(FoodContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        // โหลด relations หลัง save
        return await GetByIdAsync(order.Id) ?? order;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Restaurant)
            .Include(o => o.Chef)
            .Include(o => o.Rider)
            .Include(o => o.Items)       // <-- แทน ItemsJson เดิม
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Restaurant)
            .Include(o => o.Chef)
            .Include(o => o.Rider)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> UpdateStatusAsync(int id, string status)
    {
        var order = await GetByIdAsync(id);
        if (order == null) return null;

        order.Status = status;

        // Assign Chef เมื่อเปลี่ยนเป็น "Preparing"
        if (status == "Preparing" && order.ChefId == null)
        {
            // ค้นหา Chef ที่ว่าง (ตัวแรก)
            var availableChef = await _context.Users
                .FirstOrDefaultAsync(u => u.Role == "Chef");
            if (availableChef != null)
                order.ChefId = availableChef.Id;
        }

        // Assign Rider เมื่อเปลี่ยนเป็น "Ready"
        if (status == "Ready" && order.RiderId == null)
        {
            // ค้นหา Rider ที่ว่าง (ตัวแรก)
            var availableRider = await _context.Users
                .FirstOrDefaultAsync(u => u.Role == "Rider");
            if (availableRider != null)
                order.RiderId = availableRider.Id;
        }

        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Notification> AddNotificationAsync(int receiverId, int? orderId, string type, string message, int? restaurantId = null)
    {
        var notification = new Notification
        {
            ReceiverId = receiverId,
            OrderId = orderId,
            RestaurantId = restaurantId,
            Type = type,
            Message = message,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }
}
