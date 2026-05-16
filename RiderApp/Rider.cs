using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Text;

namespace RiderApp;

public class Rider : Form
{
    private const string ApiBase = "https://localhost:7172";

    private HubConnection? _connection;
    private readonly List<OrderSummary> _orders = new();
    private int _riderId = 1;  // ← TODO: รับจาก login form

    private FlowLayoutPanel _ordersPanel = new();
    private Label _lblStatus = new();
    private Label _lblCount = new();

    public Rider()
    {
        SetupUI();
        _ = StartupAsync();
    }

    private void SetupUI()
    {
        Text = "🛵 Rider Dashboard";
        Size = new Size(720, 620);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 250, 255);

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            BackColor = Color.FromArgb(245, 250, 255)
        };

        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));  // status bar
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));  // blue header
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // orders

        var infoPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        _lblStatus = new Label
        {
            Text = "🔄 กำลังเชื่อมต่อ...",
            AutoSize = true,
            Location = new Point(15, 13),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.Gray
        };

        _lblCount = new Label
        {
            Text = "ออเดอร์ทั้งหมด: 0 รายการ",
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(500, 13),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 50, 50)
        };

        infoPanel.Controls.Add(_lblStatus);
        infoPanel.Controls.Add(_lblCount);

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(25, 118, 210)
        };

        var title = new Label
        {
            Text = "🛵 Rider Dashboard",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.White
        };

        header.Controls.Add(title);

        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(245, 250, 255)
        };

        _ordersPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        scroll.Controls.Add(_ordersPanel);

        mainLayout.Controls.Add(infoPanel, 0, 0);
        mainLayout.Controls.Add(header, 0, 1);
        mainLayout.Controls.Add(scroll, 0, 2);

        Controls.Clear();
        Controls.Add(mainLayout);
    }

    private async Task StartupAsync()
    {
        await LoadExistingOrdersAsync();
        await ConnectSignalRAsync();
    }

    private async Task LoadExistingOrdersAsync()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{ApiBase}/api/orders");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();

            var orders = JsonSerializer.Deserialize<List<OrderSummary>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();

            Invoke(() =>
            {
                _orders.Clear();
                _orders.AddRange(orders.OrderBy(o => o.Id));
                RenderOrders();
            });
        }
        catch
        {
            Invoke(() =>
            {
                _lblStatus.Text = "🔴 โหลดข้อมูลไม่ได้";
                _lblStatus.ForeColor = Color.Red;
            });
        }
    }

    private async Task ConnectSignalRAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{ApiBase}/orderHub")
            .Build();

        _connection.On<int, string, string, List<string>, int, string>(
            "NewOrder",
            (orderId, customer, restaurantName, items, total, status) =>
            {
                Invoke(() =>
                {
                    var newOrder = new OrderSummary(
                        orderId,
                        customer,
                        restaurantName,
                        status,
                        DateTime.UtcNow,
                        items.Select(name => new OrderItemDto { FoodName = name, Quantity = 1, Price = 0 }).ToList(),
                        (decimal)total
                    );
                    // เพิ่ม Order เข้าไปแล้วเรียง
                    _orders.Add(newOrder);
                    _orders.Sort((a, b) => a.Id.CompareTo(b.Id));
                    RenderOrders();
                });
            });

        _connection.On<int, string>(
            "OrderStatusChanged",
            (orderId, status) =>
            {
                Invoke(() =>
                {
                    var index = _orders.FindIndex(o => o.Id == orderId);
                    if (index >= 0)
                    {
                        var old = _orders[index];
                        _orders[index] = old with { Status = status };
                        RenderOrders();
                    }
                });
            });

        try
        {
            await _connection.StartAsync();

            Invoke(() =>
            {
                _lblStatus.Text = "🟢 เชื่อมต่อแล้ว";
                _lblStatus.ForeColor = Color.Green;
            });
        }
        catch
        {
            Invoke(() =>
            {
                _lblStatus.Text = "🔴 เชื่อมต่อไม่ได้";
                _lblStatus.ForeColor = Color.Red;
            });
        }
    }

    private void RenderOrders()
    {
        _ordersPanel.Controls.Clear();

        foreach (var order in _orders)
        {
            _ordersPanel.Controls.Add(CreateOrderCard(order));
        }

        _lblCount.Text = $"ออเดอร์ทั้งหมด: {_orders.Count} รายการ";
    }

    private Panel CreateOrderCard(OrderSummary order)
    {
        var statusInfo = GetStatusInfo(order.Status);

        var card = new Panel
        {
            Width = 650,
            Height = 190,
            Margin = new Padding(0, 0, 0, 14),
            BackColor = Color.White
        };

        var leftBar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 8,
            BackColor = statusInfo.Color
        };

        var lblOrder = new Label
        {
            Text = $"ออเดอร์ #{order.Id}",
            Location = new Point(22, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 40, 40)
        };

        var lblRestaurant = new Label
        {
            Text = $"🏪 ร้าน: {order.RestaurantName}",
            Location = new Point(22, 45),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(46, 125, 50)
        };

        var lblCustomer = new Label
        {
            Text = $"👤 ลูกค้า: {order.CustomerName}",
            Location = new Point(22, 70),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(70, 70, 70)
        };

        var itemsText = order.Items.Count > 0
            ? string.Join(", ", order.Items)
            : "(ไม่มีรายการอาหาร)";

        var lblItems = new Label
        {
            Text = $"🍽️ รายการ: {itemsText}",
            Location = new Point(22, 95),
            Size = new Size(430, 22),
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(70, 70, 70)
        };

        var lblTotal = new Label
        {
            Text = $"฿{order.TotalPrice:N0}",
            Location = new Point(520, 20),
            Size = new Size(100, 25),
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),    
            ForeColor = Color.FromArgb(230, 81, 0)
        };

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
            Enabled = order.Status == "Ready"
        };
        btnDelivering.FlatAppearance.BorderSize = 0;

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
            Enabled = order.Status == "PickedUp"
        };
        btnDelivered.FlatAppearance.BorderSize = 0;

        if (!btnDelivering.Enabled)
            btnDelivering.BackColor = Color.Gray;

        if (!btnDelivered.Enabled)
            btnDelivered.BackColor = Color.Gray;

        btnDelivering.Click += async (s, e) =>
        {
            await UpdateOrderStatusAsync(order.Id, "PickedUp");
            await MarkNotificationsAsReadAsync(order.Id);
        };

        btnDelivered.Click += async (s, e) =>
        {
            await UpdateOrderStatusAsync(order.Id, "Delivered");
            await MarkNotificationsAsReadAsync(order.Id);
        };

        var lblTime = new Label
        {
            Text = $"🕐 {order.OrderDate.ToLocalTime():dd/MM HH:mm}",
            Location = new Point(22, 165),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.Gray
        };

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

    private async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        try
        {
            using var client = new HttpClient();

            var body = new StringContent(
                JsonSerializer.Serialize(new { status }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PatchAsync($"{ApiBase}/api/orders/{orderId}/status", body);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("อัปเดตสถานะไม่สำเร็จ");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }

    // เมธอดใหม่: ทำเครื่องหมายอ่าน notification
    private async Task MarkNotificationsAsReadAsync(int orderId)
    {
        try
        {
            // ดึง notifications ของ rider นี้
            using var client = new HttpClient();
            var response = await client.GetAsync($"{ApiBase}/api/notifications/user/{_riderId}");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var notifications = JsonSerializer.Deserialize<List<NotificationDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            // ทำเครื่องหมายอ่านแล้วสำหรับ notifications ที่ยังไม่ได้อ่าน
            foreach (var notif in notifications.Where(n => n.OrderId == orderId && !n.IsRead))
            {
                using var markClient = new HttpClient();
                await markClient.PatchAsync($"{ApiBase}/api/notifications/{notif.Id}/read", null);
            }
        }
        catch { /* ไม่สำคัญถ้าไม่สำเร็จ */ }
    }

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

public record OrderSummary(
    int Id,
    string CustomerName,
    string RestaurantName,
    string Status,
    DateTime OrderDate,        // ← เปลี่ยนจาก CreatedAt
    List<OrderItemDto> Items,  // ← เปลี่ยนจาก List<string>
    decimal TotalPrice         // ← เปลี่ยนจาก int Total
);

public class OrderItemDto
{
    public string FoodName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public override string ToString() => $"{FoodName}";
}

// DTO สำหรับ Notification
public class NotificationDto
{
    public int Id { get; set; }
    public int ReceiverId { get; set; }
    public int? OrderId { get; set; }
    public string Type { get; set; } = "";
    public string Message { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
}