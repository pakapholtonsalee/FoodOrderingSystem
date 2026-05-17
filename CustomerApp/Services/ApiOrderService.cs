using System.Text;
using System.Text.Json;
using CustomerApp.Models;

namespace CustomerApp.Services;

/// <summary>
/// Concrete implementation ของ IOrderService ที่ติดต่อกับ FoodApi ผ่าน HTTP
///
/// Facade Pattern: รวม HTTP logic ที่เคยกระจายอยู่ใน 2 ที่มาไว้ที่เดียว
///   เดิม: btnOrder_Click ใน Customer.cs มีโค้ด PostAsync อยู่
///   เดิม: LoadOrdersAsync ใน OrderStatus.cs มีโค้ด GetAsync อยู่
///   ตอนนี้: ทั้งสองรวมกันที่นี่ Form แค่เรียก method สั้นๆ
/// </summary>
public class ApiOrderService : IOrderService
{
    // URL ของ FoodApi endpoint
    // TODO: ย้ายไปไว้ใน appsettings หรือ environment variable แทน hardcode
    private const string BaseUrl = "https://localhost:7172/api/orders";

    // ตั้งค่า JSON Deserializer ให้ไม่ case-sensitive
    // เช่น API ส่ง "orderId" → map กับ property "OrderId" ได้เลย
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ใช้ static HttpClient 1 ตัวร่วมกันทั้ง app (best practice)
    // ถ้า new HttpClient() ทุกครั้งที่เรียก จะเกิด socket exhaustion ได้
    private static readonly HttpClient _http = new();

    /// <summary>
    /// สร้างและส่งออเดอร์ไปยัง FoodApi ด้วย HTTP POST
    /// </summary>
    public async Task<int> PlaceOrderAsync(int restaurantId, IEnumerable<CartItem> items)
    {
        // สร้าง payload ในรูปแบบที่ FoodApi คาดหวัง (anonymous object → JSON)
        var payload = new
        {
            customerId = 1,        // TODO: เปลี่ยนเป็น id จริงหลัง implement login
            restaurantId = restaurantId,
            status = "Pending",
            items = items.Select(x => new
            {
                foodName = x.Name,
                quantity = 1,
                price = (decimal)x.Price
            })
        };

        // แปลง object → JSON string → StringContent พร้อมส่ง
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // ส่ง POST request ไปยัง API
        var response = await _http.PostAsync(BaseUrl, content);
        response.EnsureSuccessStatusCode(); // ถ้า status 4xx/5xx จะ throw HttpRequestException

        // อ่าน response JSON แล้วดึงแค่ "id" ออกมาคืนให้ caller
        // ตัวอย่าง response: { "id": 42, "status": "Pending", ... }
        var resultJson = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(resultJson);
        return doc.RootElement.GetProperty("id").GetInt32();
    }

    /// <summary>
    /// ดึงรายการออเดอร์ทั้งหมดจาก FoodApi ด้วย HTTP GET
    /// </summary>
    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        var response = await _http.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        // Deserialize JSON array → List<OrderDto>
        // ถ้า API คืนค่า null หรือ response ว่าง → คืน empty list แทน
        return JsonSerializer.Deserialize<List<OrderDto>>(json, JsonOptions) ?? new();
    }
}
