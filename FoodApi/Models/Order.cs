using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApi.Models;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("customer_id")]
    public int? CustomerId { get; set; }
    public User? Customer { get; set; }

    [Column("restaurant_id")]
    public int? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    [Column("chef_id")]
    public int? ChefId { get; set; }
    public User? Chef { get; set; }

    [Column("rider_id")]
    public int? RiderId { get; set; }
    public User? Rider { get; set; }

    [Column("status")]
    public string Status { get; set; } = "Pending";

    [Column("total_price")]
    public decimal TotalPrice { get; set; } = 0;

    [Column("order_date")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public List<OrderItem> Items { get; set; } = new();
}