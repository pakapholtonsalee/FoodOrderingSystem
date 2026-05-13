using FoodApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApi.Data;

public class FoodContext : DbContext
{
    public FoodContext(DbContextOptions<FoodContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
}