using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApi.Models;

[Table("restaurants")]
public class Restaurant
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("delivery_time")]
    public string? DeliveryTime { get; set; }   // PostgreSQL เป็น VARCHAR(50)

    [Column("rating")]
    public double Rating { get; set; }
}