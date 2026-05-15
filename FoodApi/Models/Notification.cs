using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodApi.Models;

[Table("notifications")]
public class Notification
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("receiver_id")]
    public int ReceiverId { get; set; }
    public User? Receiver { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    [Column("restaurant_id")]
    public int? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    [Column("type")]
    public string Type { get; set; } = "order_status";

    [Column("message")]
    public string Message { get; set; } = "";

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("sent_at")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}