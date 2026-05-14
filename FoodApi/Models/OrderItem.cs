namespace FoodApi.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public string FoodName { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }
}