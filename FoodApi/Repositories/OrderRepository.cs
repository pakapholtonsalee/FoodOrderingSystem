using FoodApi.Data;
using FoodApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApi.Repositories;

/// <summary>
/// จัดการ database สำหรับออเดอร์ (Repository Pattern)
/// แยก logic การเข้าถึง DB ออกจาก Controller
/// </summary>
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
        return order;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
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