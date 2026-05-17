namespace CustomerApp.Models;

/// <summary>
/// DTO (Data Transfer Object) สำหรับรับข้อมูลออเดอร์จาก FoodApi
///
/// DTO คืออะไร?
///   เป็น class ที่ใช้รับ/ส่งข้อมูลกับ API เท่านั้น
///   ไม่มี business logic — แค่เก็บข้อมูลที่ API ส่งกลับมา
///   ApiOrderService จะ Deserialize JSON → OrderDto แล้วส่งให้ OrderStatus ใช้แสดงผล
/// </summary>
public class OrderDto
{
    public int Id { get; set; }                    // หมายเลขออเดอร์ ใช้ highlight ออเดอร์ของ user
    public string CustomerName { get; set; } = ""; // ชื่อลูกค้า
    public int RestaurantId { get; set; }           // รหัสร้าน
    public string RestaurantName { get; set; } = "";
    public string Status { get; set; } = "Pending"; // สถานะปัจจุบัน เช่น "preparing", "delivered"
    public DateTime OrderDate { get; set; }         // วันเวลาที่สั่ง ใช้ sort และแสดงใน card
    public decimal TotalPrice { get; set; }         // ราคารวม
    public List<OrderItemDto> Items { get; set; } = new(); // รายการอาหารในออเดอร์นี้
}

/// <summary>
/// DTO สำหรับแต่ละรายการอาหารในออเดอร์
/// ToString() override เพื่อให้แสดงแค่ชื่ออาหารตอน join เป็น string
/// </summary>
public class OrderItemDto
{
    public int Id { get; set; }
    public string FoodName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    // ถูกเรียกตอน string.Join(", ", order.Items) ใน OrderStatus
    // ผลลัพธ์: "🍕 Margherita Pizza, 🥤 Cola"
    public override string ToString() => FoodName;
}
