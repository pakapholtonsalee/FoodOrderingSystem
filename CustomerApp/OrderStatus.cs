using System.Text.Json;
using System.Drawing.Drawing2D;

namespace CustomerApp;

public partial class OrderStatus : Form
{
    private readonly int _highlightOrderId;
    private readonly string _restaurantName;
    private System.Windows.Forms.Timer? _refreshTimer;
    private FlowLayoutPanel? _ordersPanel;
    private Label? _lblLastUpdate;

    public OrderStatus(int highlightOrderId, string restaurantName)
    {
        _highlightOrderId = highlightOrderId;
        _restaurantName = restaurantName;
        InitializeComponent();
        SetupUI();
        this.Shown += async (s, e) => await LoadOrdersAsync();

        // Auto-refresh every 2 seconds
        _refreshTimer = new System.Windows.Forms.Timer { Interval = 2000 };
        _refreshTimer.Tick += async (s, e) => await LoadOrdersAsync();
        _refreshTimer.Start();
    }

    void SetupUI()
    {
        this.Text = "📊 ติดตามสถานะออเดอร์";
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(620, 580);
        this.MinimumSize = new Size(560, 480);

        // Scrollable orders panel (add first so it fills the space)
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(245, 245, 250),
            Padding = new Padding(16, 12, 16, 12),
        };

        _ordersPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Transparent,
        };

        scroll.Controls.Add(_ordersPanel);
        this.Controls.Add(scroll);

        // Toolbar (add before legend so it's on top)
        var toolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 46,
            BackColor = Color.White,
            Padding = new Padding(12, 7, 12, 7),
        };

        _lblLastUpdate = new Label
        {
            Text = "🔄 กำลังโหลด...",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            AutoSize = true,
            Location = new Point(12, 14),
        };

        var btnRefresh = new Button
        {
            Text = "🔄 รีเฟรช",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(63, 81, 181),
            FlatStyle = FlatStyle.Flat,
            Size = new Size(95, 30),
            Cursor = Cursors.Hand,
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += async (s, e) => await LoadOrdersAsync();

        toolbar.Controls.Add(btnRefresh);
        toolbar.Controls.Add(_lblLastUpdate);

        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.Location = new Point(toolbar.Width - 107, 7);
        toolbar.Resize += (s, e) => btnRefresh.Location = new Point(toolbar.Width - 107, 7);

        this.Controls.Add(toolbar);

        // Legend
        var legendPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            BackColor = Color.FromArgb(230, 232, 255),
            Padding = new Padding(12, 8, 12, 8),
        };

        var legendFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
        };

        legendFlow.Controls.Add(MakeLegendChip("⏳ รอรับออเดอร์", Color.FromArgb(255, 152, 0)));
        legendFlow.Controls.Add(MakeLegendChip("👨‍🍳 กำลังจัดเตรียม", Color.FromArgb(33, 150, 243)));
        legendFlow.Controls.Add(MakeLegendChip("✅ เสร็จแล้ว", Color.FromArgb(76, 175, 80)));
        legendPanel.Controls.Add(legendFlow);
        this.Controls.Add(legendPanel);

        // Header (add last so it's on top)
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 75,
            BackColor = Color.FromArgb(63, 81, 181),
        };

        var lblTitle = new Label
        {
            Text = "📊  สถานะออเดอร์",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        header.Controls.Add(lblTitle);
        this.Controls.Add(header);
    }

    private Label MakeLegendChip(string text, Color color)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = color,
            AutoSize = true,
            Margin = new Padding(0, 0, 16, 0),
            TextAlign = ContentAlignment.MiddleLeft,
        };
    }

    private async Task LoadOrdersAsync()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7172/api/orders");

            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            if (this.IsDisposed) return;

            this.Invoke(() =>
            {
                RenderOrders(orders);
                if (_lblLastUpdate != null)
                    _lblLastUpdate.Text = $"🕐 อัปเดตล่าสุด: {DateTime.Now:HH:mm:ss}";
            });
        }
        catch
        {
            if (!this.IsDisposed)
                this.Invoke(() =>
                {
                    if (_lblLastUpdate != null)
                        _lblLastUpdate.Text = "❌ เชื่อมต่อ server ไม่ได้";
                });
        }
    }

    private void RenderOrders(List<OrderDto> orders)
    {
        if (_ordersPanel == null) return;
        _ordersPanel.SuspendLayout();
        _ordersPanel.Controls.Clear();

        if (orders.Count == 0)
        {
            var lbl = new Label
            {
                Text = "ยังไม่มีออเดอร์",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(20),
            };
            _ordersPanel.Controls.Add(lbl);
        }

        // Show most recent first
        foreach (var order in orders.OrderByDescending(o => o.CreatedAt))
        {
            var card = CreateOrderCard(order);
            _ordersPanel.Controls.Add(card);
        }

        _ordersPanel.Width = _ordersPanel.Parent?.ClientSize.Width - 32 ?? 560;
        _ordersPanel.ResumeLayout();
    }

    private Panel CreateOrderCard(OrderDto order)
    {
        var isHighlight = order.Id == _highlightOrderId;
        var statusInfo = GetStatusInfo(order.Status);

        var cardWidth = _ordersPanel?.Parent?.ClientSize.Width - 48 ?? 540;
        var card = new Panel
        {
            Width = cardWidth > 100 ? cardWidth : 480,
            Height = 110,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Color.White,
            Cursor = Cursors.Default,
        };

        card.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Card shadow
            using var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
            g.FillRectangle(shadowBrush, 2, 2, card.Width - 2, card.Height - 2);

            // White bg
            var rect = new Rectangle(0, 0, card.Width - 3, card.Height - 3);
            g.FillRectangle(Brushes.White, rect);

            // Left status bar
            using var barBrush = new SolidBrush(statusInfo.Color);
            g.FillRectangle(barBrush, 0, 0, 6, card.Height - 3);

            // Highlight order
            if (isHighlight)
            {
                using var highlightBrush = new SolidBrush(Color.FromArgb(15, 63, 81, 181));
                g.FillRectangle(highlightBrush, rect);
                using var hlPen = new Pen(Color.FromArgb(63, 81, 181), 1.5f);
                g.DrawRectangle(hlPen, rect);
            }
            else
            {
                using var borderPen = new Pen(Color.FromArgb(225, 225, 230));
                g.DrawRectangle(borderPen, rect);
            }
        };

        // Order ID + highlight badge
        var orderTitle = isHighlight
            ? $"ออเดอร์ #{order.Id}  ★ ออเดอร์ของคุณ"
            : $"ออเดอร์ #{order.Id}";

        var lblId = new Label
        {
            Text = orderTitle,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = isHighlight ? Color.FromArgb(63, 81, 181) : Color.FromArgb(40, 40, 40),
            Location = new Point(16, 12),
            AutoSize = true,
            BackColor = Color.Transparent,
        };
        card.Controls.Add(lblId);

        // Items
        var itemsText = order.Items != null && order.Items.Count > 0
            ? string.Join(", ", order.Items)
            : "(ไม่มีรายการ)";

        var lblItems = new Label
        {
            Text = "🍽️  " + itemsText,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(90, 90, 90),
            Location = new Point(16, 38),
            Size = new Size(card.Width - 160, 18),
            BackColor = Color.Transparent,
        };
        card.Controls.Add(lblItems);

        // Total
        var lblTotal = new Label
        {
            Text = $"💰 ฿{order.Total:N0}",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(180, 60, 20),
            Location = new Point(16, 60),
            AutoSize = true,
            BackColor = Color.Transparent,
        };
        card.Controls.Add(lblTotal);

        // Created at
        var localTime = order.CreatedAt.ToLocalTime();
        var lblTime = new Label
        {
            Text = "🕐 " + localTime.ToString("dd/MM HH:mm"),
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.Gray,
            Location = new Point(16, 83),
            AutoSize = true,
            BackColor = Color.Transparent,
        };
        card.Controls.Add(lblTime);

        // Status badge (right side)
        var statusBadge = new Panel
        {
            Size = new Size(130, 42),
            BackColor = Color.Transparent,
        };
        statusBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        statusBadge.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, statusBadge.Width - 1, statusBadge.Height - 1);
            using var bg = new SolidBrush(Color.FromArgb(30, statusInfo.Color));
            using var path = GetRoundedRect(rect, 8);
            g.FillPath(bg, path);
            using var border = new Pen(statusInfo.Color, 1.5f);
            g.DrawPath(border, path);
        };

        var lblStatus = new Label
        {
            Text = statusInfo.Emoji + " " + statusInfo.Label,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = statusInfo.Color,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };
        statusBadge.Controls.Add(lblStatus);

        // Position status badge
        card.Controls.Add(statusBadge);
        card.Resize += (s, e) =>
        {
            statusBadge.Location = new Point(card.Width - 145, 20);
            lblItems.Width = card.Width - 160;
        };
        statusBadge.Location = new Point(card.Width - 145, 20);

        // Progress bar area
        if (order.Status != "Pending")
        {
            var progressPanel = new Panel
            {
                Location = new Point(16, 82),
                Size = new Size(card.Width - 170, 14),
                BackColor = Color.Transparent,
            };

            int progress = order.Status == "Preparing" ? 50 : 100;

            progressPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var trackRect = new Rectangle(0, 4, progressPanel.Width, 6);
                using var trackBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
                g.FillRectangle(trackBrush, trackRect);

                int fillW = (int)(progressPanel.Width * progress / 100.0);
                if (fillW > 0)
                {
                    using var fillBrush = new SolidBrush(statusInfo.Color);
                    g.FillRectangle(fillBrush, 0, 4, fillW, 6);
                }
            };

            card.Controls.Add(progressPanel);
        }

        return card;
    }

    private static GraphicsPath GetRoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static (string Emoji, string Label, Color Color) GetStatusInfo(string status)
    {
        return status?.ToLower() switch
        {
            "preparing" => ("👨‍🍳", "กำลังจัดเตรียม", Color.FromArgb(33, 150, 243)),
            "ready" or "completed" or "done" => ("✅", "เสร็จแล้ว", Color.FromArgb(76, 175, 80)),
            _ => ("⏳", "รอรับออเดอร์", Color.FromArgb(255, 152, 0)),
        };
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        base.OnFormClosed(e);
    }
}

// DTO สำหรับรับข้อมูลออเดอร์จาก API
public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public int RestaurantId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public List<string> Items { get; set; } = new();   // รับเป็น List<string> ตรงๆ
    public int Total { get; set; }
}