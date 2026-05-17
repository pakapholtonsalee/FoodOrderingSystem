using Microsoft.AspNetCore.SignalR.Client;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RiderApp;

// Rider Form หลักของระบบ Rider
// ใช้แสดงรายการออเดอร์ และอัปเดตสถานะแบบ real-time ผ่าน SignalR
public class Rider : Form
{
    // URL หลักของ Backend API
    // ใช้สำหรับเรียก API และเชื่อม SignalR Hub
    private const string ApiBase = "https://localhost:7172";

    // ตัวแปรสำหรับเชื่อม SignalR
    // ใช้รับข้อมูล real-time จาก Server
    private HubConnection? _connection;

    // เก็บรายการออเดอร์ทั้งหมดที่แสดงบนหน้าจอ
    private readonly List<OrderSummary> _orders = new();

    // Rider Id ปัจจุบัน
    // ตอนนี้กำหนดเป็นค่า fix ไว้ก่อน ยังไม่ได้เชื่อมระบบ Login จริง
    private int _riderId = 1;  // ← TODO: รับจาก login form

    // Panel สำหรับแสดงรายการ order card
    private FlowLayoutPanel _ordersPanel = new();

    // Label แสดงสถานะการเชื่อมต่อ SignalR
    private Label _lblStatus = new();

    // Label แสดงจำนวนออเดอร์ทั้งหมด
    private Label _lblCount = new();

    // Constructor ของ Rider Form
    public Rider()
    {
        // สร้าง UI ทั้งหมดของหน้าจอ
        SetupUI();

        // เริ่มโหลดข้อมูลและเชื่อม SignalR แบบ asynchronous
        _ = StartupAsync();
    }

    // สร้าง UI ของ Rider Dashboard
    private void SetupUI()
    {
        // กำหนดชื่อหน้าต่าง
        Text = "🛵 Rider Dashboard";

        // กำหนดขนาดหน้าต่าง
        Size = new Size(720, 620);

        // ให้หน้าต่างเปิดตรงกลางหน้าจอ
        StartPosition = FormStartPosition.CenterScreen;

        // สีพื้นหลังหลักของ Form
        BackColor = Color.FromArgb(245, 250, 255);

        // Layout หลักของหน้าจอ
        // ใช้ TableLayoutPanel เพื่อแบ่งพื้นที่เป็น 3 ส่วน
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            BackColor = Color.FromArgb(245, 250, 255)
        };

        // แถวที่ 1 = status bar
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

        // แถวที่ 2 = blue header
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

        // แถวที่ 3 = รายการออเดอร์
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Panel แสดงข้อมูลสถานะด้านบน
        var infoPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        // Label แสดงสถานะการเชื่อมต่อ SignalR
        _lblStatus = new Label
        {
            Text = "🔄 กำลังเชื่อมต่อ...",
            AutoSize = true,
            Location = new Point(15, 13),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.Gray
        };

        // Label แสดงจำนวน order ทั้งหมด
        _lblCount = new Label
        {
            Text = "ออเดอร์ทั้งหมด: 0 รายการ",
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(500, 13),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 50, 50)
        };

        // เพิ่ม label ลง panel
        infoPanel.Controls.Add(_lblStatus);
        infoPanel.Controls.Add(_lblCount);

        // Header สีฟ้าของระบบ
        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(25, 118, 210)
        };

        // Title หลักของ Rider Dashboard
        var title = new Label
        {
            Text = "🛵 Rider Dashboard",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.White
        };

        header.Controls.Add(title);

        // Panel สำหรับ scroll รายการออเดอร์
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(245, 250, 255)
        };

        // Panel หลักที่ใช้เก็บ order card หลายใบ
        // ใช้ FlowLayoutPanel เพื่อเรียง card จากบนลงล่าง
        _ordersPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        scroll.Controls.Add(_ordersPanel);

        // เพิ่ม component ทั้งหมดลง layout หลัก
        mainLayout.Controls.Add(infoPanel, 0, 0);
        mainLayout.Controls.Add(header, 0, 1);
        mainLayout.Controls.Add(scroll, 0, 2);

        // ล้าง control เดิมก่อน
        Controls.Clear();

        // เพิ่ม layout หลักลง Form
        Controls.Add(mainLayout);
    }

    // เมธอดเริ่มต้นของระบบ
    // ใช้โหลดข้อมูลเก่าและเชื่อม SignalR
    private async Task StartupAsync()
    {
        // โหลดออเดอร์ทั้งหมดจากฐานข้อมูลก่อน
        await LoadExistingOrdersAsync();

        // จากนั้นเชื่อม real-time ผ่าน SignalR
        await ConnectSignalRAsync();
    }

    // โหลดออเดอร์ทั้งหมดจาก API
    private async Task LoadExistingOrdersAsync()
    {
        try
        {
            using var client = new HttpClient();

            // เรียก API ดึงรายการ order ทั้งหมด
            var response = await client.GetAsync($"{ApiBase}/api/orders");

            // ถ้า API ตอบไม่สำเร็จให้ออกจากเมธอด
            if (!response.IsSuccessStatusCode) return;

            // อ่าน JSON จาก response
            var json = await response.Content.ReadAsStringAsync();

            // แปลง JSON เป็น List<OrderSummary>
            var orders = JsonSerializer.Deserialize<List<OrderSummary>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();

            // Invoke ใช้สำหรับอัปเดต UI จาก thread อื่น
            Invoke(() =>
            {
                // ล้างรายการเก่า
                _orders.Clear();

                // เพิ่มรายการใหม่และเรียงตาม Order Id
                _orders.AddRange(orders.OrderBy(o => o.Id));

                // วาดรายการ order ใหม่
                RenderOrders();
            });
        }
        catch
        {
            // กรณีโหลดข้อมูลไม่สำเร็จ
            Invoke(() =>
            {
                _lblStatus.Text = "🔴 โหลดข้อมูลไม่ได้";
                _lblStatus.ForeColor = Color.Red;
            });
        }
    }

    // เชื่อมต่อ SignalR Hub
    // ใช้รับข้อมูลแบบ real-time
    private async Task ConnectSignalRAsync()
    {
        // สร้าง SignalR Connection
        _connection = new HubConnectionBuilder()
            .WithUrl($"{ApiBase}/orderHub")
            .Build();
        // 2. Observer Pattern ผ่าน SignalR
        // Event เมื่อมีออเดอร์ใหม่เข้าระบบ
        _connection.On<int, string, string, List<string>, int, string>(
            "NewOrder",
            (orderId, customer, restaurantName, items, total, status) =>
            {
                Invoke(() =>
                {
                    // สร้าง object OrderSummary ใหม่
                    var newOrder = new OrderSummary(
                        orderId,
                        customer,
                        restaurantName,
                        status,
                        DateTime.UtcNow,
                        items.Select(name => new OrderItemDto { FoodName = name, Quantity = 1, Price = 0 }).ToList(),
                        (decimal)total
                    );

                    // เพิ่ม order ใหม่เข้า list
                    _orders.Add(newOrder);

                    // เรียงตาม Order Id
                    _orders.Sort((a, b) => a.Id.CompareTo(b.Id));

                    // วาด UI ใหม่
                    RenderOrders();
                });
            });

        // Event เมื่อสถานะ order ถูกเปลี่ยน
        _connection.On<int, string>(
            "OrderStatusChanged",
            (orderId, status) =>
            {
                Invoke(() =>
                {
                    // หา order ที่ต้องการอัปเดต
                    var index = _orders.FindIndex(o => o.Id == orderId);

                    if (index >= 0)
                    {
                        var old = _orders[index];

                        // ใช้ record with expression เพื่อสร้าง object ใหม่
                        // โดยเปลี่ยนเฉพาะ Status
                        _orders[index] = old with { Status = status };

                        // render UI ใหม่
                        RenderOrders();
                    }
                });
            });

        try
        {
            // เริ่มเชื่อมต่อ SignalR
            await _connection.StartAsync();

            // แสดงสถานะเชื่อมต่อสำเร็จ
            Invoke(() =>
            {
                _lblStatus.Text = "🟢 เชื่อมต่อแล้ว";
                _lblStatus.ForeColor = Color.Green;
            });
        }
        catch
        {
            // กรณีเชื่อมต่อไม่สำเร็จ
            Invoke(() =>
            {
                _lblStatus.Text = "🔴 เชื่อมต่อไม่ได้";
                _lblStatus.ForeColor = Color.Red;
            });
        }
    }

    // วาดรายการออเดอร์ทั้งหมดใหม่
    private void RenderOrders()
    {
        // ล้าง card เดิมทั้งหมด
        _ordersPanel.Controls.Clear();

        // สร้าง card ใหม่ทีละ order
        foreach (var order in _orders)
        {
            _ordersPanel.Controls.Add(CreateOrderCard(order));
        }

        // อัปเดตจำนวน order
        _lblCount.Text = $"ออเดอร์ทั้งหมด: {_orders.Count} รายการ";
    }

    // สร้าง Order Card สำหรับแต่ละออเดอร์
    private Panel CreateOrderCard(OrderSummary order)
    {
        // ดึงข้อความและสีตามสถานะ order
        var statusInfo = GetStatusInfo(order.Status);

        // Card หลักของออเดอร์
        var card = new Panel
        {
            Width = 650,
            Height = 190,
            Margin = new Padding(0, 0, 0, 14),
            BackColor = Color.White
        };

        // แถบสีด้านซ้ายของ card
        // สีเปลี่ยนตามสถานะ order
        var leftBar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 8,
            BackColor = statusInfo.Color
        };

        // Label แสดงเลขออเดอร์
        var lblOrder = new Label
        {
            Text = $"ออเดอร์ #{order.Id}",
            Location = new Point(22, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 40, 40)
        };

        // Label แสดงชื่อร้านอาหาร
        var lblRestaurant = new Label
        {
            Text = $"🏪 ร้าน: {order.RestaurantName}",
            Location = new Point(22, 45),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(46, 125, 50)
        };

        // Label แสดงชื่อลูกค้า
        var lblCustomer = new Label
        {
            Text = $"👤 ลูกค้า: {order.CustomerName}",
            Location = new Point(22, 70),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(70, 70, 70)
        };

        // ตรวจสอบว่ามีรายการอาหารหรือไม่
        var itemsText = order.Items.Count > 0
            ? string.Join(", ", order.Items)
            : "(ไม่มีรายการอาหาร)";

        // Label แสดงรายการอาหาร
        var lblItems = new Label
        {
            Text = $"🍽️ รายการ: {itemsText}",
            Location = new Point(22, 95),
            Size = new Size(430, 22),
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(70, 70, 70)
        };

        // Label แสดงราคารวม
        var lblTotal = new Label
        {
            Text = $"฿{order.TotalPrice:N0}",
            Location = new Point(520, 20),
            Size = new Size(100, 25),
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.FromArgb(230, 81, 0)
        };

        // Label แสดงสถานะปัจจุบันของ order
        var lblStatus = new Label
        {
            Text = statusInfo.Text,
            Location = new Point(455, 60),
            Size = new Size(165, 35),
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = statusInfo.Color
        };

        // ปุ่มเปลี่ยนสถานะเป็น PickedUp
        // จะกดได้เฉพาะเมื่อร้านทำอาหารเสร็จแล้ว
        var btnDelivering = new Button
        {
            Text = "🛵 กำลังเดินทางจัดส่งอาหาร",
            Location = new Point(22, 125),
            Size = new Size(270, 38),
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(46, 125, 50),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,

            // ปุ่มเปิดได้เฉพาะ status = Ready
            Enabled = order.Status == "Ready"
        };

        btnDelivering.FlatAppearance.BorderSize = 0;

        // ปุ่มเปลี่ยนสถานะเป็น Delivered
        var btnDelivered = new Button
        {
            Text = "✅ จัดส่งสำเร็จ",
            Location = new Point(310, 125),
            Size = new Size(180, 38),
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(76, 175, 80),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,

            // ปุ่มเปิดได้เฉพาะเมื่อ Rider รับอาหารแล้ว
            Enabled = order.Status == "PickedUp"
        };

        btnDelivered.FlatAppearance.BorderSize = 0;

        // ถ้าปุ่มถูก disable ให้เปลี่ยนเป็นสีเทา
        if (!btnDelivering.Enabled)
            btnDelivering.BackColor = Color.Gray;

        if (!btnDelivered.Enabled)
            btnDelivered.BackColor = Color.Gray;

        //1.Event - Driven Programming
        // Event เมื่อกดปุ่มกำลังจัดส่ง
        btnDelivering.Click += async (s, e) =>
        {
            // เปลี่ยนสถานะ order เป็น PickedUp
            await UpdateOrderStatusAsync(order.Id, "PickedUp");

            // ทำ notification เป็นอ่านแล้ว
            await MarkNotificationsAsReadAsync(order.Id);
        };

        // Event เมื่อกดปุ่มจัดส่งสำเร็จ
        btnDelivered.Click += async (s, e) =>
        {
            // เปลี่ยนสถานะเป็น Delivered
            await UpdateOrderStatusAsync(order.Id, "Delivered");

            // ทำ notification เป็นอ่านแล้ว
            await MarkNotificationsAsReadAsync(order.Id);
        };

        // Label แสดงเวลา order
        var lblTime = new Label
        {
            Text = $"🕐 {order.OrderDate.ToLocalTime():dd/MM HH:mm}",
            Location = new Point(22, 165),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.Gray
        };

        // เพิ่ม control ทั้งหมดลง card
        card.Controls.Add(leftBar);
        card.Controls.Add(lblOrder);
        card.Controls.Add(lblRestaurant);
        card.Controls.Add(lblCustomer);
        card.Controls.Add(lblItems);
        card.Controls.Add(lblTotal);
        card.Controls.Add(lblStatus);
        card.Controls.Add(btnDelivering);
        card.Controls.Add(btnDelivered);
        card.Controls.Add(lblTime);

        return card;
    }

    // เรียก API เพื่อเปลี่ยนสถานะ order
    private async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        try
        {
            using var client = new HttpClient();

            // สร้าง JSON body สำหรับส่งไป API
            var body = new StringContent(
                JsonSerializer.Serialize(new { status }),
                Encoding.UTF8,
                "application/json"
            );

            // ใช้ PATCH เพราะแก้เฉพาะ field status
            var response = await client.PatchAsync($"{ApiBase}/api/orders/{orderId}/status", body);

            // กรณีอัปเดตไม่สำเร็จ
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("อัปเดตสถานะไม่สำเร็จ");
            }
        }
        catch (Exception ex)
        {
            // แสดง error ที่เกิดขึ้น
            MessageBox.Show("Error: " + ex.Message);
        }
    }

    // เมธอดสำหรับทำเครื่องหมายอ่าน Notification
    private async Task MarkNotificationsAsReadAsync(int orderId)
    {
        try
        {
            // ดึง notifications ของ Rider คนปัจจุบัน
            using var client = new HttpClient();

            var response = await client.GetAsync($"{ApiBase}/api/notifications/user/{_riderId}");

            if (!response.IsSuccessStatusCode) return;

            // อ่าน JSON จาก API
            var json = await response.Content.ReadAsStringAsync();

            // แปลง JSON เป็น NotificationDto
            var notifications = JsonSerializer.Deserialize<List<NotificationDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            // เลือก notification ที่เป็นของ order นี้
            // และยังไม่ได้อ่าน
            foreach (var notif in notifications.Where(n => n.OrderId == orderId && !n.IsRead))
            {
                using var markClient = new HttpClient();

                // PATCH API เพื่อ mark as read
                await markClient.PatchAsync($"{ApiBase}/api/notifications/{notif.Id}/read", null);
            }
        }
        catch
        {
            // ถ้า notification fail ไม่ให้ระบบล่ม
        }
    }

    // แปลงสถานะ order เป็นข้อความและสี
    // ใช้สำหรับแสดงผลบน UI
    private static (string Text, Color Color) GetStatusInfo(string status)
    {
        return status?.ToLower() switch
        {
            "preparing" => ("👨‍🍳 กำลังจัดเตรียม", Color.FromArgb(33, 150, 243)),
            "ready" => ("✅ ร้านทำเสร็จแล้ว", Color.FromArgb(76, 175, 80)),
            "pickedup" => ("🛵 กำลังเดินทางจัดส่งอาหาร", Color.FromArgb(46, 125, 50)),
            "delivered" => ("🎉 จัดส่งสำเร็จ", Color.FromArgb(27, 94, 32)),
            _ => ("⏳ รอรับออเดอร์", Color.FromArgb(255, 152, 0))
        };
    }
}
//3. DTO / Data Transfer Object Pattern
// Model สำหรับเก็บข้อมูล order
public record OrderSummary(
    int Id,
    string CustomerName,
    string RestaurantName,
    string Status,

    // วันที่สร้างออเดอร์
    DateTime OrderDate,

    // รายการอาหารทั้งหมด
    List<OrderItemDto> Items,

    // ราคารวมทั้งหมด
    decimal TotalPrice
);

// DTO สำหรับรายการอาหาร
public class OrderItemDto
{
    // ชื่ออาหาร
    public string FoodName { get; set; } = "";

    // จำนวนอาหาร
    public int Quantity { get; set; }

    // ราคาอาหาร
    public decimal Price { get; set; }

    // ใช้แสดงชื่ออาหารตอน string.Join()
    public override string ToString() => $"{FoodName}";
}

// DTO สำหรับ Notification
public class NotificationDto
{
    // Notification Id
    public int Id { get; set; }

    // User ผู้รับ Notification
    public int ReceiverId { get; set; }

    // ออเดอร์ที่เกี่ยวข้อง
    public int? OrderId { get; set; }

    // ประเภท Notification
    public string Type { get; set; } = "";

    // ข้อความ Notification
    public string Message { get; set; } = "";

    // ตรวจสอบว่าอ่านแล้วหรือยัง
    public bool IsRead { get; set; }

    // เวลาที่ส่ง Notification
    public DateTime SentAt { get; set; }
}