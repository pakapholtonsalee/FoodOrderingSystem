using System.Drawing.Drawing2D;
using CustomerApp.Data;
using CustomerApp.Helpers;
using CustomerApp.Models;
using CustomerApp.Services;

namespace CustomerApp;

/// <summary>
/// Form แรกที่เปิดขึ้นมา — แสดงรายการร้านอาหารให้เลือก
///
/// Flow: Program.cs → ShopSelect → (เลือกร้าน) → Customer form
///
/// Design Patterns ที่ใช้:
///   Repository Pattern  : ดึงข้อมูลร้านจาก RestaurantRepository ไม่ hardcode เอง
///   Factory Method      : CreateRestaurantCard() สร้าง card ทีละร้าน
///   Dependency Injection: สร้าง CartService + ApiOrderService แล้วส่งให้ Customer
/// </summary>
public partial class ShopSelect : Form
{
    public ShopSelect()
    {
        InitializeComponent(); // โหลด Designer.cs (ถ้ามี control ที่ drag-drop ไว้)
        this.Text = "🍽️ Food Order - เลือกร้านอาหาร";
        this.BackColor = Color.FromArgb(232, 245, 233);
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    // OnLoad เรียกหลัง constructor เสร็จ และก่อน Form แสดงบนหน้าจอ
    // เหมาะกว่าใส่ใน constructor เพราะ Form size พร้อมแล้ว
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        BuildUI();
    }

    /// <summary>สร้าง UI ทั้งหน้าตอน load — header, subtitle, และ card ของแต่ละร้าน</summary>
    private void BuildUI()
    {
        // Repository Pattern: ดึงข้อมูลร้านทั้งหมด — ไม่รู้ว่ามาจาก hardcode หรือ API
        var restaurants = RestaurantRepository.GetAll();

        // FlowLayoutPanel จัด card ให้เรียงแนวตั้งอัตโนมัติ ไม่ต้องคำนวณ y ของแต่ละ card เอง
        var cardsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(232, 245, 233),
            Padding = new Padding(30, 20, 30, 20),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true, // ถ้ามีร้านเยอะเกิน Form จะ scroll ได้
        };

        // Factory Method: สร้าง card panel สำเร็จรูปให้แต่ละร้าน
        foreach (var restaurant in restaurants)
            cardsPanel.Controls.Add(CreateRestaurantCard(restaurant));

        this.Controls.Add(cardsPanel);

        // Subtitle แสดงจำนวนร้าน
        this.Controls.Add(new Label
        {
            Text = $"มีร้านให้เลือก {restaurants.Count} ร้าน",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(120, 80, 50),
            AutoSize = false,
            Height = 30,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(46, 125, 50),
        });

        // Factory Method จาก UIStyler: สร้าง header panel สำเร็จรูป
        this.Controls.Add(
            UIStyler.CreateHeaderPanel(Color.FromArgb(255, 87, 34), 90, "🍽️  เลือกร้านอาหาร", 22));
    }

    /// <summary>
    /// เปิด Customer form สำหรับร้านที่เลือก
    /// Dependency Injection: สร้าง CartService และ ApiOrderService ใหม่ต่อ session
    /// แล้วส่งเข้า constructor ของ Customer — Customer ไม่ต้อง new เองข้างใน
    /// </summary>
    private void OpenRestaurant(Restaurant restaurant)
    {
        var cart = new CartService();    // ตะกร้าใหม่ทุกครั้งที่เข้าร้าน
        var orderService = new ApiOrderService(); // HTTP client สำหรับคุยกับ FoodApi
        var form = new Customer(restaurant, cart, orderService);

        // เมื่อ Customer form ปิด → กลับมาแสดง ShopSelect
        form.FormClosed += (_, _) => this.Show();
        this.Hide(); // ซ่อนแทนปิด เพื่อให้กลับมาได้
        form.Show();
    }

    /// <summary>
    /// Factory Method: สร้าง Panel แสดงข้อมูลร้าน 1 การ์ด
    /// รวม background drawing, labels, และปุ่ม "เลือก" ไว้ในที่เดียว
    /// </summary>
    private Panel CreateRestaurantCard(Restaurant restaurant)
    {
        var card = new Panel
        {
            Width = 480,
            Height = 140,
            Margin = new Padding(0, 0, 0, 16), // ระยะห่างระหว่างแต่ละการ์ด
            BackColor = Color.White,
            Cursor = Cursors.Hand,
        };

        // ใช้ Paint event วาด background แทน BackColor เพราะต้องการ gradient + shadow
        card.Paint += (_, e) => DrawCardBackground(e.Graphics, card, restaurant);

        // ปุ่ม "เลือก ›" มุมขวาล่างของการ์ด
        var btnEnter = new Button
        {
            Text = "เลือก  ›",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = restaurant.BackColor1, // สีธีมของร้าน
            FlatStyle = FlatStyle.Flat,
            Location = new Point(385, 88),
            Size = new Size(80, 28),
            Cursor = Cursors.Hand,
        };
        btnEnter.FlatAppearance.BorderSize = 0;
        btnEnter.Click += (_, _) => OpenRestaurant(restaurant);

        // Labels แสดงข้อมูลร้าน (ตำแหน่ง x=155 เพราะ 0-140 เป็นแถบสีของร้าน)
        var lblEmoji = MakeLabel(restaurant.Name.Split(' ')[0], 32, Color.White,
                            new Point(10, 30), new Size(120, 60), ContentAlignment.MiddleCenter);
        var lblName = MakeLabel(string.Join(" ", restaurant.Name.Split(' ').Skip(1)), 14,
                            Color.FromArgb(40, 40, 40), new Point(155, 18), new Size(300, 28), bold: true);
        var lblDesc = MakeLabel(restaurant.Description, 9.5f,
                            Color.FromArgb(100, 100, 100), new Point(155, 48), new Size(300, 20));
        var lblTag = MakeLabel("🏷️  " + restaurant.Tag, 8.5f,
                            Color.FromArgb(150, 100, 50), new Point(155, 72), new Size(200, 18));
        var lblRating = MakeLabel($"⭐ {restaurant.Rating:0.0}", 9,
                            Color.FromArgb(200, 140, 0), new Point(155, 95), new Size(90, 20), bold: true);
        var lblTime = MakeLabel("🕐 " + restaurant.DeliveryTime, 9,
                            Color.FromArgb(80, 140, 80), new Point(250, 95), new Size(150, 20));

        // คลิกที่ card หรือ label สำคัญได้ ไม่จำเป็นต้องกดปุ่มเท่านั้น
        foreach (Control ctrl in new Control[] { card, lblEmoji, lblName, lblDesc })
            ctrl.Click += (_, _) => OpenRestaurant(restaurant);

        card.Controls.AddRange(new Control[] { lblEmoji, lblName, lblDesc, lblTag, lblRating, lblTime, btnEnter });
        btnEnter.BringToFront(); // ให้ปุ่มอยู่บนสุด ไม่ถูก label ทับ

        return card;
    }

    /// <summary>
    /// วาด background ของ card:
    ///   1. Shadow เล็กน้อย (สีดำโปร่งแสง offset ลงขวา)
    ///   2. พื้นขาว
    ///   3. แถบสีร้าน (gradient ด้านซ้าย 140px)
    ///   4. กรอบเทาอ่อน
    /// </summary>
    private static void DrawCardBackground(Graphics g, Panel card, Restaurant restaurant)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias; // วาดเส้นโค้งให้เรียบ

        // 1. Shadow
        using var shadow = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
        g.FillRectangle(shadow, 3, 3, card.Width - 3, card.Height - 3);

        // 2. พื้นขาว
        var rect = new Rectangle(0, 0, card.Width - 4, card.Height - 4);
        g.FillRectangle(Brushes.White, rect);

        // 3. แถบสีร้าน gradient จากซ้ายไปขวา
        using var strip = new LinearGradientBrush(
            new Point(0, 0), new Point(160, 0),
            restaurant.BackColor1, restaurant.BackColor2);
        g.FillRectangle(strip, 0, 0, 140, card.Height - 4);

        // 4. กรอบ
        using var pen = new Pen(Color.FromArgb(230, 230, 230));
        g.DrawRectangle(pen, rect);
    }

    /// <summary>Helper สร้าง Label โดยไม่ต้องระบุ property ซ้ำทุกครั้ง</summary>
    private static Label MakeLabel(string text, float size, Color color,
        Point loc, Size sz, ContentAlignment align = ContentAlignment.TopLeft, bool bold = false)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = color,
            Location = loc,
            Size = sz,
            TextAlign = align,
            BackColor = Color.Transparent, // โปร่งใสเพื่อให้เห็น background card ข้างหลัง
        };
    }

}
