using Microsoft.AspNetCore.SignalR.Client;
using System.Text;
using System.Text.Json;

namespace RestaurantApp;

public partial class Restaurant : Form
{
    private const string ApiBase = "https://localhost:7172";

    HubConnection? _connection;
    private readonly List<(int orderId, string text)> _orders = new();

    public Restaurant()
    {
        InitializeComponent();
        this.Text = "🍽️ Restaurant Dashboard";
        this.BackColor = System.Drawing.Color.FromArgb(245, 250, 245);
        _ = StartupAsync();
    }

    // โหลดออเดอร์เก่าจาก API แล้วเชื่อม SignalR
    async Task StartupAsync()
    {
        await LoadExistingOrdersAsync();
        await ConnectSignalRAsync();
    }

    // ดึงออเดอร์ทั้งหมดจาก DB (เพื่อดูออเดอร์เก่าได้)
    async Task LoadExistingOrdersAsync()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{ApiBase}/api/orders");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderSummary>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            Invoke(() =>
            {
                foreach (var o in orders)
                    AddOrderToList(o.Id, o.CustomerName, o.Items, o.TotalPrice);
            });
        }
        catch { /* ไม่สามารถโหลดออเดอร์เก่าได้ ข้ามไป */ }
    }

    async Task ConnectSignalRAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{ApiBase}/orderHub")
            .Build();

        // รับออเดอร์ใหม่ real-time
        _connection.On<int, string, string, List<string>, int, string>("NewOrder",
            (orderId, customer, restaurantName, items, total, status) =>
                Invoke(() => AddOrderToList(
                    orderId,
                    customer,
                    items.Select(name => new OrderItemDto { FoodName = name, Quantity = 1, Price = 0 }).ToList(),
                    (decimal)total
                )));

        // รับการเปลี่ยนสถานะ real-time
        _connection.On<int, string>("OrderStatusChanged",
            (orderId, status) =>
                Invoke(() => UpdateOrderStatus(orderId, status)));

        try
        {
            await _connection.StartAsync();
            Invoke(() =>
            {
                lblStatus.Text = "🟢 เชื่อมต่อแล้ว";
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
            });
        }
        catch
        {
            Invoke(() =>
            {
                lblStatus.Text = "🔴 เชื่อมต่อไม่ได้";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            });
        }
    }

    // เพิ่มออเดอร์เข้า list (ใช้ทั้งตอนโหลดเก่า และรับ real-time)
    void AddOrderToList(int orderId, string customer, List<OrderItemDto> items, decimal total)
    {
        var itemsText = items.Count > 0
            ? string.Join(", ", items.Select(i => $"{i.FoodName} x{i.Quantity}"))
            : "(ไม่มีรายการ)";
        var text = $"ออเดอร์ #{orderId}  |  {customer}  |  {itemsText}  |  ฿{total}";
        _orders.Add((orderId, text));
        listBoxOrders.Items.Add(text);
        lblOrderCount.Text = $"ออเดอร์ทั้งหมด: {_orders.Count} รายการ";
    }

    // อัปเดตสถานะใน list
    void UpdateOrderStatus(int orderId, string status)
    {
        var statusLabel = status switch
        {
            "Preparing" => "👨‍🍳 กำลังจัดเตรียม",
            "Completed" => "✅ เสร็จแล้ว",
            "Delivering" => "🛵 กำลังเดินทางจัดส่งอาหาร",
            "Delivered" => "🎉 จัดส่งสำเร็จ",
            _ => "⏳ รอรับออเดอร์",
        };

        for (int i = 0; i < _orders.Count; i++)
        {
            if (_orders[i].orderId != orderId) continue;

            // ลบ tag สถานะเก่าออกก่อน แล้วใส่อันใหม่
            var baseText = _orders[i].text.Split(" | [")[0];
            var newText = $"{baseText} | [{statusLabel}]";
            _orders[i] = (orderId, newText);
            listBoxOrders.Items[i] = newText;
            break;
        }
    }

    private async void btnPreparing_Click(object sender, EventArgs e)
        => await UpdateSelectedOrderStatus("Preparing");

    private async void btnCompleted_Click(object sender, EventArgs e)
        => await UpdateSelectedOrderStatus("Completed");

    private async Task UpdateSelectedOrderStatus(string status)
    {
        if (listBoxOrders.SelectedIndex < 0 || listBoxOrders.SelectedIndex >= _orders.Count)
        {
            MessageBox.Show("กรุณาเลือกออเดอร์ก่อน");
            return;
        }

        var orderId = _orders[listBoxOrders.SelectedIndex].orderId;

        try
        {
            using var client = new HttpClient();
            var body = new StringContent(
                JsonSerializer.Serialize(new { status }),
                Encoding.UTF8, "application/json");

            var response = await client.PatchAsync($"{ApiBase}/api/orders/{orderId}/status", body);

            if (!response.IsSuccessStatusCode)
                MessageBox.Show("อัปเดตสถานะไม่สำเร็จ");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}

// DTO สำหรับรับข้อมูลออเดอร์จาก API
// แก้ record ด้านล่างไฟล์
record OrderSummary(
    int Id,
    string CustomerName,
    List<OrderItemDto> Items,  // ← เปลี่ยนจาก List<string>
    decimal TotalPrice         // ← เปลี่ยนจาก int Total
);

public class OrderItemDto
{
    public string FoodName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}