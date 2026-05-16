using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApi.Models;

[Table("order_items")]
public class OrderItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    [Column("food_name")]
    public string FoodName { get; set; } = "";

    [Column("quantity")]
    public int Quantity { get; set; } = 1;

    [Column("price")]
    public decimal Price { get; set; }
}