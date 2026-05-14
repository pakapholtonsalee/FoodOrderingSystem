using FoodApi.Models;

namespace FoodApi.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> UpdateStatusAsync(int id, string status);
}