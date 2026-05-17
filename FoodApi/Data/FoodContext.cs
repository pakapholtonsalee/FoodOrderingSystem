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
        // Order -> User (Customer)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Order -> User (Chef)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Chef)
            .WithMany()
            .HasForeignKey(o => o.ChefId)
            .OnDelete(DeleteBehavior.SetNull);

        // Order -> User (Rider)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Rider)
            .WithMany()
            .HasForeignKey(o => o.RiderId)
            .OnDelete(DeleteBehavior.SetNull);

        // Order -> Restaurant
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Restaurant)
            .WithMany()
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.SetNull);

        // OrderItem -> Order
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification -> User
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Receiver)
            .WithMany()
            .HasForeignKey(n => n.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification -> Order
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Order)
            .WithMany()
            .HasForeignKey(n => n.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification -> Restaurant
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Restaurant)
            .WithMany()
            .HasForeignKey(n => n.RestaurantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}