namespace CustomerApp.Models;

/// <summary>
/// Model เก็บข้อมูลร้านอาหาร 1 ร้าน
/// ถูกสร้างและเก็บไว้ใน RestaurantRepository แล้วส่งต่อให้ ShopSelect และ Customer ใช้
/// </summary>
public class Restaurant
{
    public int Id { get; set; }                  // ใช้ส่งไปบอก API ว่าสั่งจากร้านไหน
    public string Name { get; set; } = "";        // ชื่อร้าน เช่น "🍕 Pizza Palace"
    public string Description { get; set; } = ""; // คำอธิบายร้าน แสดงบน card
    public string Tag { get; set; } = "";          // ประเภทอาหาร เช่น "Italian · Fastfood"
    public string DeliveryTime { get; set; } = ""; // เวลาส่งโดยประมาณ เช่น "25–35 นาที"
    public float Rating { get; set; }              // คะแนนร้าน แสดงดาว ⭐

    // สีธีมของร้าน — ใช้ทำ gradient บน card และเป็นสี header ใน Customer form
    public Color BackColor1 { get; set; }
    public Color BackColor2 { get; set; }

    // เมนูอาหาร: key = ชื่อเมนู, value = ราคา (บาท)
    // ใช้ Dictionary เพราะแต่ละร้านมีเมนูไม่เหมือนกัน
    public Dictionary<string, int> Menu { get; set; } = new();
}
