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
        await _context.SaveChangesAsync();
        return order;
    }
}
