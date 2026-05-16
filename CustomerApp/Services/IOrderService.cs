using CustomerApp.Models;

namespace CustomerApp.Services;

/// <summary>
/// Interface กำหนด "สัญญา" ว่า Service ที่ทำงานกับออเดอร์ต้องมี method อะไรบ้าง
///
/// Facade Pattern: ซ่อนรายละเอียด HTTP ทั้งหมดไว้ข้างหลัง interface นี้
///   Form ต่างๆ (Customer, OrderStatus) รู้จักแค่ interface นี้
///   ไม่รู้ว่าข้างในใช้ HttpClient, JSON, หรือ URL อะไร
///
/// Dependency Injection: ShopSelect สร้าง ApiOrderService แล้วส่งผ่าน interface นี้
///   ทำให้ถ้าอยากเปลี่ยนไปใช้ MockOrderService สำหรับ test → แค่สร้าง class ใหม่ที่ implements interface นี้
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// ส่งออเดอร์ไปยัง FoodApi
    /// คืนค่า orderId ที่ได้รับจาก API เพื่อนำไปแสดงผลและ highlight ใน OrderStatus
    /// </summary>
    Task<int> PlaceOrderAsync(int restaurantId, IEnumerable<CartItem> items);

    /// <summary>
    /// ดึงรายการออเดอร์ทั้งหมดจาก FoodApi
    /// ถูกเรียกซ้ำทุก 2 วินาทีโดย Timer ใน OrderStatus เพื่อ refresh สถานะ
    /// </summary>
    Task<List<OrderDto>> GetOrdersAsync();
}
