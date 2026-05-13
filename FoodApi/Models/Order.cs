using System.Text.Json;

namespace FoodApi.Models;

public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = "";

    public int RestaurantId { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }

    // เก็บเป็น JSON string ใน DB แทน List<string>
    public string ItemsJson { get; set; } = "[]";

    // Property ที่ใช้ใน code ปกติ (ไม่เก็บใน DB)
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public List<string> Items
    {
        get => JsonSerializer.Deserialize<List<string>>(ItemsJson) ?? new();
        set => ItemsJson = JsonSerializer.Serialize(value);
    }

    public int Total { get; set; }
}
