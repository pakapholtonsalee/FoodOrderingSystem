using System.Drawing.Drawing2D;
using CustomerApp.Helpers;
using CustomerApp.Models;
using CustomerApp.Services;

namespace CustomerApp;
/*
    Flow note (ที่มาของข้อมูลร้านอาหารจาก RestaurantRepository):
    - RestaurantRepository.GetAll() ถูกเรียกโดย ShopSelect เพื่อสร้างการ์ดร้าน
    - เมื่อผู้ใช้เลือกร้าน ShopSelect.OpenRestaurant จะสร้าง Customer form และส่งวัตถุ Restaurant ให้
    - Customer จะแสดงเมนูจาก Restaurant.Menu และเมื่อสั่งอาหารจะเรียก ApiOrderService.PlaceOrderAsync
    - ApiOrderService ส่งคำสั่งสั่งอาหารเป็น HTTP POST ไปยัง FoodApi (/api/orders)
    - FoodApi บันทึก Order ลงฐานข้อมูล และกระจายเหตุการณ์แบบ real-time ผ่าน SignalR (NewOrder, OrderStatusChanged)
    - แอพอื่นๆ (เช่น RestaurantApp, RiderApp) รับเหตุการณ์ SignalR และอัปเดต UI แบบเรียลไทม์
    - OrderStatus form ใช้ ApiOrderService.GetOrdersAsync ดึงรายการออเดอร์จาก API แล้วแสดงสถานะโดยใช้ StatusResolver
*/

/// <summary>
/// Form ติดตามสถานะออเดอร์ — auto-refresh ทุก 2 วินาที
///
/// Flow: Customer → OrderStatus (เปิดหลังสั่งอาหารสำเร็จ หรือกดปุ่ม "ดูสถานะ")
///
/// Design Patterns ที่ใช้:
///   Dependency Injection: รับ IOrderService ผ่าน constructor ไม่ new เอง
///   Strategy Pattern    : ใช้ StatusResolver แทน switch-case ที่กระจัดกระจาย
///   Factory Method      : CreateOrderCard() สร้าง card สำเร็จรูปทีละออเดอร์
/// </summary>
public partial class OrderStatus : Form
{
    // ── Fields ────────────────────────────────────────────────────────

    private readonly int _highlightOrderId; // orderId ที่ต้อง highlight (ออเดอร์ของ user)
    private readonly string _restaurantName;   // ชื่อร้าน (เก็บไว้สำหรับใช้ในอนาคต)
    private readonly IOrderService _orderService;     // ใช้ GetOrdersAsync() ทุก 2 วิ

    private System.Windows.Forms.Timer? _refreshTimer; // Timer ที่ทำให้ refresh อัตโนมัติ
    private FlowLayoutPanel? _ordersPanel;  // Panel ที่ใส่ order card ทั้งหมด
    private Label? _lblLastUpdate; // แสดงเวลา refresh ล่าสุด

    /// <summary>
    /// Constructor รับ orderId ที่เพิ่ง place ไป เพื่อ highlight ใน list
    /// ถ้าเปิดจากปุ่ม "ดูสถานะ" (ไม่ได้เพิ่งสั่ง) จะส่ง 0 มา → ไม่มี highlight
    /// </summary>
    public OrderStatus(int highlightOrderId, string restaurantName, IOrderService orderService)
    {
        _highlightOrderId = highlightOrderId;
        _restaurantName = restaurantName;
        _orderService = orderService;

        InitializeComponent();
        SetupUI();

        // Shown event ทำงานหลัง Form แสดงบนหน้าจอแล้ว → ดึงข้อมูลครั้งแรก
        this.Shown += async (_, _) => await RefreshAsync();

        // ตั้ง Timer ให้ refresh ทุก 2000ms (2 วินาที)
        _refreshTimer = new System.Windows.Forms.Timer { Interval = 2000 };
        _refreshTimer.Tick += async (_, _) => await RefreshAsync();
        _refreshTimer.Start();
    }

    // ── UI Setup ──────────────────────────────────────────────────────

    private void SetupUI()
    {
        this.Text = "📊 ติดตามสถานะออเดอร์";
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(620, 580);
        this.MinimumSize = new Size(560, 480); // ป้องกัน Form เล็กเกินจน card อ่านไม่ออก

        // scroll panel ครอบ _ordersPanel เพื่อให้ scroll ได้เมื่อมีออเดอร์เยอะ
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(245, 245, 250),
            Padding = new Padding(16, 12, 16, 12),
        };

        // _ordersPanel: FlowLayout เรียง card แนวตั้ง ขนาดปรับตาม content อัตโนมัติ
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

        // เพิ่ม Toolbar และ Legend ก่อน header (Dock = Top เรียงจากบนลงล่างตามลำดับที่ Add)
        this.Controls.Add(BuildToolbar());
        this.Controls.Add(BuildLegend());
        this.Controls.Add(
            UIStyler.CreateHeaderPanel(Color.FromArgb(63, 81, 181), 75, "📊  สถานะออเดอร์"));
    }

    /// <summary>สร้าง toolbar แถบบนสุด — มีปุ่ม "🔄 รีเฟรช" และ label แสดงเวลา update ล่าสุด</summary>
    private Panel BuildToolbar()
    {
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

        // สร้างปุ่มจาก UIStyler (Factory Method) แล้วปรับ size เพิ่มเติม
        var btnRefresh = UIStyler.CreateStyledButton("🔄 รีเฟรช", Color.FromArgb(63, 81, 181));
        btnRefresh.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        btnRefresh.Size = new Size(95, 30);
        btnRefresh.Height = 30;
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right; // ชิดขวา
        btnRefresh.Click += async (_, _) => await RefreshAsync();

        toolbar.Controls.Add(btnRefresh);
        toolbar.Controls.Add(_lblLastUpdate);

        // ทุกครั้งที่ toolbar ขยาย/หด → คำนวณตำแหน่งปุ่มใหม่ให้ชิดขวาเสมอ
        toolbar.Resize += (_, _) =>
            btnRefresh.Location = new Point(toolbar.Width - 107, 7);

        return toolbar;
    }

    /// <summary>
    /// สร้าง legend แถบแสดงความหมายของแต่ละสถานะ
    /// ใช้ StatusResolver.Resolve() เพื่อดึง emoji + สี — ไม่ hardcode ซ้ำ
    /// </summary>
    private static Panel BuildLegend()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            BackColor = Color.FromArgb(230, 232, 255),
            Padding = new Padding(12, 8, 12, 8),
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
        };

        // วนสร้าง label ทุก status โดยดึงข้อมูลจาก StatusResolver
        // ถ้าเพิ่ม status ใหม่ใน StatusResolver → legend อัปเดตอัตโนมัติ
        foreach (var status in new[] { "pending", "preparing", "ready", "delivering", "delivered" })
        {
            var info = StatusResolver.Resolve(status); // Strategy Pattern
            flow.Controls.Add(new Label
            {
                Text = $"{info.Emoji} {info.Label}",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = info.Color,
                AutoSize = true,
                Margin = new Padding(0, 0, 16, 0),
                TextAlign = ContentAlignment.MiddleLeft,
            });
        }

        panel.Controls.Add(flow);
        return panel;
    }

    // ── Data Loading ──────────────────────────────────────────────────

    /// <summary>
    /// ดึงออเดอร์จาก API แล้วอัปเดต UI
    /// ถูกเรียกตอน Form โหลดครั้งแรก และทุก 2 วิโดย _refreshTimer
    /// </summary>
    private async Task RefreshAsync()
    {
        try
        {
            // await: ไม่บล็อก UI thread ขณะรอ HTTP response
            var orders = await _orderService.GetOrdersAsync();
            if (this.IsDisposed) return; // Form อาจถูกปิดระหว่างรอ

            // this.Invoke: บังคับรัน code ใน UI thread (WinForms rules)
            // ห้ามแก้ UI controls จาก background thread
            this.Invoke(() =>
            {
                RenderOrders(orders);
                if (_lblLastUpdate != null)
                    _lblLastUpdate.Text = $"🕐 อัปเดตล่าสุด: {DateTime.Now:HH:mm:ss}";
            });
        }
        catch
        {
            // แสดง error message ใน UI thread
            if (!this.IsDisposed)
                this.Invoke(() =>
                {
                    if (_lblLastUpdate != null)
                        _lblLastUpdate.Text = "❌ เชื่อมต่อ server ไม่ได้";
                });
        }
    }

    /// <summary>
    /// ล้าง order cards เดิมทั้งหมดแล้วสร้างใหม่จาก list ที่ได้จาก API
    /// SuspendLayout/ResumeLayout ป้องกัน UI กระพริบระหว่าง redraw
    /// </summary>
    private void RenderOrders(List<OrderDto> orders)
    {
        if (_ordersPanel == null) return;

        _ordersPanel.SuspendLayout(); // หยุด layout engine ชั่วคราว
        _ordersPanel.Controls.Clear(); // ลบ card เก่าทั้งหมด

        if (orders.Count == 0)
        {
            _ordersPanel.Controls.Add(new Label
            {
                Text = "ยังไม่มีออเดอร์",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(20),
            });
        }

        // เรียงออเดอร์ใหม่สุดขึ้นก่อน แล้วสร้าง card ให้แต่ละออเดอร์
        foreach (var order in orders.OrderByDescending(o => o.OrderDate))
            _ordersPanel.Controls.Add(CreateOrderCard(order));

        // ปรับ width ของ _ordersPanel ให้เต็ม scroll panel
        _ordersPanel.Width = _ordersPanel.Parent?.ClientSize.Width - 32 ?? 560;
        _ordersPanel.ResumeLayout(); // เปิด layout engine กลับ → วาด UI ครั้งเดียว
    }

    // ── Factory Method: สร้าง order card ─────────────────────────────

    /// <summary>
    /// Factory Method: สร้าง Panel แสดงข้อมูล 1 ออเดอร์
    /// ถ้า order.Id ตรงกับ _highlightOrderId จะมี highlight สีน้ำเงิน
    /// </summary>
    private Panel CreateOrderCard(OrderDto order)
    {
        var isHighlight = order.Id == _highlightOrderId; // ออเดอร์ของ user หรือเปล่า?
        var statusInfo = StatusResolver.Resolve(order.Status); // Strategy: แปลง status → emoji/color
        var cardWidth = _ordersPanel?.Parent?.ClientSize.Width - 48 ?? 540;

        var card = new Panel
        {
            Width = cardWidth > 100 ? cardWidth : 480,
            Height = 110,
            Margin = new Padding(0, 0, 0, 10), // ระยะห่างระหว่าง card
            BackColor = Color.White,
        };

        // ใช้ Paint event วาด background เพื่อให้มี shadow, color bar, และ highlight
        card.Paint += (_, e) => DrawCardBackground(e.Graphics, card, statusInfo, isHighlight);

        // แถว 1: Order ID (ถ้าเป็นออเดอร์ของ user จะมี ★)
        card.Controls.Add(MakeLabel(
            isHighlight ? $"ออเดอร์ #{order.Id}  ★ ออเดอร์ของคุณ" : $"ออเดอร์ #{order.Id}",
            11, isHighlight ? Color.FromArgb(63, 81, 181) : Color.FromArgb(40, 40, 40),
            new Point(16, 12), autoSize: true, bold: true));

        // แถว 2: รายการอาหาร — join ด้วย ", " เช่น "🍕 Margherita, 🥤 Cola"
        var itemsText = order.Items?.Count > 0
            ? string.Join(", ", order.Items) // OrderItemDto.ToString() คืน FoodName
            : "(ไม่มีรายการ)";
        card.Controls.Add(MakeLabel("🍽️  " + itemsText, 9,
            Color.FromArgb(90, 90, 90), new Point(16, 38), new Size(card.Width - 160, 18)));

        // แถว 3: ราคารวม
        card.Controls.Add(MakeLabel($"💰 ฿{order.TotalPrice:N0}", 9.5f,
            Color.FromArgb(180, 60, 20), new Point(16, 60), autoSize: true, bold: true));

        // แถว 4: เวลาที่สั่ง
        card.Controls.Add(MakeLabel(
            "🕐 " + order.OrderDate.ToLocalTime().ToString("dd/MM HH:mm"),
            8.5f, Color.Gray, new Point(16, 83), autoSize: true));

        // Status badge มุมขวาบน — ปรับตำแหน่งตาม card width เมื่อ resize
        var badge = CreateStatusBadge(statusInfo);
        card.Controls.Add(badge);
        badge.Location = new Point(card.Width - 145, 20);
        card.Resize += (_, _) => badge.Location = new Point(card.Width - 145, 20);

        // Progress bar แถวล่างสุด — แสดงเฉพาะถ้าไม่ใช่ Pending (progress > 0)
        int progress = StatusResolver.GetProgress(order.Status);
        if (progress > 0)
            card.Controls.Add(CreateProgressBar(card, progress, statusInfo.Color));

        return card;
    }

    /// <summary>
    /// วาด background ของ order card:
    ///   - Shadow เล็กน้อย
    ///   - แถบสีสถานะ 6px ทางซ้าย (บ่งบอกสถานะด้วยสีทันที)
    ///   - ถ้า highlight: พื้นน้ำเงินอ่อนและกรอบน้ำเงิน
    ///   - ถ้าไม่ highlight: กรอบเทาอ่อน
    /// </summary>
    private static void DrawCardBackground(Graphics g, Panel card, StatusInfo statusInfo, bool isHighlight)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Shadow
        using var shadow = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
        g.FillRectangle(shadow, 2, 2, card.Width - 2, card.Height - 2);

        var rect = new Rectangle(0, 0, card.Width - 3, card.Height - 3);
        g.FillRectangle(Brushes.White, rect);

        // แถบสีสถานะ 6px ทางซ้าย — สีมาจาก StatusResolver
        using var bar = new SolidBrush(statusInfo.Color);
        g.FillRectangle(bar, 0, 0, 6, card.Height - 3);

        if (isHighlight)
        {
            // พื้นน้ำเงินโปร่งแสง + กรอบน้ำเงิน
            using var hlBrush = new SolidBrush(Color.FromArgb(15, 63, 81, 181));
            g.FillRectangle(hlBrush, rect);
            using var hlPen = new Pen(Color.FromArgb(63, 81, 181), 1.5f);
            g.DrawRectangle(hlPen, rect);
        }
        else
        {
            using var border = new Pen(Color.FromArgb(225, 225, 230));
            g.DrawRectangle(border, rect);
        }
    }

    /// <summary>
    /// สร้าง status badge รูปสี่เหลี่ยมมุมมน มุมขวาบนของ card
    /// วาดด้วย GraphicsHelper.RoundedRect() + สีจาก StatusInfo
    /// </summary>
    private static Panel CreateStatusBadge(StatusInfo statusInfo)
    {
        var badge = new Panel
        {
            Size = new Size(130, 42),
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };

        badge.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, badge.Width - 1, badge.Height - 1);

            // พื้นหลัง badge โปร่งแสง (opacity 30 จาก 255)
            using var bg = new SolidBrush(Color.FromArgb(30, statusInfo.Color));
            using var path = GraphicsHelper.RoundedRect(rect, 8); // มุมมน radius=8
            g.FillPath(bg, path);

            // กรอบ badge
            using var pen = new Pen(statusInfo.Color, 1.5f);
            g.DrawPath(pen, path);
        };

        // Label แสดง Emoji + ชื่อสถานะ กลาง badge
        badge.Controls.Add(new Label
        {
            Text = $"{statusInfo.Emoji} {statusInfo.Label}",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = statusInfo.Color,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        });

        return badge;
    }

    /// <summary>
    /// สร้าง progress bar แสดงความคืบหน้าของออเดอร์
    /// วาดด้วย Paint event:
    ///   1. แถบเทา = track ทั้งหมด
    ///   2. แถบสี = ส่วนที่คืบหน้าแล้ว (คำนวณจาก % × width)
    /// </summary>
    private static Panel CreateProgressBar(Panel card, int progress, Color color)
    {
        var panel = new Panel
        {
            Location = new Point(16, 82),
            Size = new Size(card.Width - 170, 14),
            BackColor = Color.Transparent,
        };

        panel.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Track (พื้นหลัง): เทาอ่อน ความสูง 6px
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 220, 220)),
                new Rectangle(0, 4, panel.Width, 6));

            // Fill (ความคืบหน้า): สีตามสถานะ
            int fillW = (int)(panel.Width * progress / 100.0);
            if (fillW > 0)
                g.FillRectangle(new SolidBrush(color), 0, 4, fillW, 6);
        };

        return panel;
    }

    /// <summary>Helper สร้าง Label โดยไม่ต้องระบุ property ซ้ำทุกครั้ง</summary>
    private static Label MakeLabel(string text, float size, Color color, Point loc,
        Size sz = default, bool autoSize = false, bool bold = false) => new()
        {
            Text = text,
            Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = color,
            Location = loc,
            Size = sz == default ? new Size(100, 20) : sz,
            AutoSize = autoSize,
            BackColor = Color.Transparent,
        };

    /// <summary>
    /// เมื่อปิด Form ต้อง Stop และ Dispose Timer ด้วย
    /// ถ้าไม่ทำ Timer จะยังวิ่งอยู่ทำให้ memory leak และ error เมื่อ Invoke กลับมาที่ Form ที่ปิดแล้ว
    /// </summary>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        base.OnFormClosed(e);
    }
}
