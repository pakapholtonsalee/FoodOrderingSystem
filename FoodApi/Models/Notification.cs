namespace FoodApi.Models;

public class Notification
{
    public int Id { get; set; }

    public int ReceiverId { get; set; }
    public User? Receiver { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public string Type { get; set; } = "order_status";
    public string Message { get; set; } = "";
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}