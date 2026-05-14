namespace FoodApi.Models;

public class Order
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }
    public User? Customer { get; set; }

    public int? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    public int? ChefId { get; set; }
    public User? Chef { get; set; }

    public int? RiderId { get; set; }
    public User? Rider { get; set; }

    public string Status { get; set; } = "Pending";
    public decimal TotalPrice { get; set; } = 0;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    // Navigation property
    public List<OrderItem> Items { get; set; } = new();
}