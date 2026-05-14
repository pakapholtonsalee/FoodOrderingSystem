using FoodApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodApi.Data;

public class FoodContext : DbContext
{
    public FoodContext(DbContextOptions<FoodContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<Order>().Property(o => o.Id).HasColumnName("id");
        modelBuilder.Entity<Order>().Property(o => o.CustomerId).HasColumnName("customer_id");
        modelBuilder.Entity<Order>().Property(o => o.RestaurantId).HasColumnName("restaurant_id");
        modelBuilder.Entity<Order>().Property(o => o.ChefId).HasColumnName("chef_id");
        modelBuilder.Entity<Order>().Property(o => o.RiderId).HasColumnName("rider_id");
        modelBuilder.Entity<Order>().Property(o => o.TotalPrice).HasColumnName("total_price");
        modelBuilder.Entity<Order>().Property(o => o.OrderDate).HasColumnName("order_date");

        modelBuilder.Entity<OrderItem>().ToTable("order_items");
        modelBuilder.Entity<OrderItem>().Property(i => i.OrderId).HasColumnName("order_id");
        // existing FK configs can remain

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("restaurants");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("restaurants");
            entity.Property(e => e.Id).HasColumnName("id");
        });
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasColumnName("status");
    }
}