using FoodApi.Controllers;
using FoodApi.Models;

namespace FoodApi.Services;

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(CreateOrderRequest request);
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order?> ChangeOrderStatusAsync(int id, string newStatus);
}