using System.Collections.Generic;

namespace FoodApi.Models;

public class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = "";

    public int RestaurantId { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }

    public List<string> Items { get; set; } = new();

    public int Total { get; set; }
}