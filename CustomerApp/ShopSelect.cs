using System.Drawing.Drawing2D;

namespace CustomerApp;

public partial class ShopSelect : Form
{
    public ShopSelect()
    {
        InitializeComponent();
        this.Text = "🍽️ Food Order - เลือกร้านอาหาร";
        this.BackColor = Color.FromArgb(232, 245, 233);
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    // ข้อมูลร้านอาหาร (สามารถดึงจาก API ได้ในอนาคต)
    static readonly Restaurant[] Restaurants = new[]
    {
        new Restaurant
        {
            Id = 1,
            Name = "🍕 Pizza Palace",
            Description = "พิซซ่าเตาฟืน อิตาเลียนแท้ๆ",
            Tag = "Italian · Fastfood",
            DeliveryTime = "25–35 นาที",
            Rating = 4.8f,
            BackColor1 = Color.FromArgb(76, 175, 80),
            BackColor2 = Color.FromArgb(46, 125, 50),
            Menu = new Dictionary<string, int>
            {
                { "🍕 Margherita Pizza", 199 },
                { "🍕 Pepperoni Pizza", 229 },
                { "🍕 BBQ Chicken Pizza", 249 },
                { "🥤 Cola", 39 },
                { "🥗 Caesar Salad", 89 },
            }
        },
        new Restaurant
        {
            Id = 2,
            Name = "🍔 Burger Bros",
            Description = "เบอร์เกอร์เนื้อวากิว พรีเมียม",
            Tag = "American · Grill",
            DeliveryTime = "20–30 นาที",
            Rating = 4.6f,
            BackColor1 = Color.FromArgb(76, 175, 80),
            BackColor2 = Color.FromArgb(46, 125, 50),
            Menu = new Dictionary<string, int>
            {
                { "🍔 Classic Burger", 129 },
                { "🍔 Double Cheese Burger", 169 },
                { "🍔 Wagyu Burger", 299 },
                { "🍟 French Fries", 59 },
                { "🥤 Milkshake", 89 },
            }
        },
    };

    private void OpenRestaurant(Restaurant restaurant)
    {
        var customerForm = new Customer(restaurant);
        customerForm.FormClosed += (s, e) => this.Show();
        this.Hide();
        customerForm.Show();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Cards panel (add first so it fills the space)
        var cardsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(232, 245, 233),
            Padding = new Padding(30, 20, 30, 20),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
        };

        foreach (var restaurant in Restaurants)
        {
            var card = CreateRestaurantCard(restaurant);
            cardsPanel.Controls.Add(card);
        }

        this.Controls.Add(cardsPanel);

        // Subtitle (add before header so it's on top)
        var lblSub = new Label
        {
            Text = "มีร้านให้เลือก " + Restaurants.Length + " ร้าน",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(120, 80, 50),
            AutoSize = false,
            Height = 30,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(46, 125, 50),
        };
        this.Controls.Add(lblSub);

        // Header Panel (add last so it stays on top)
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.FromArgb(255, 87, 34),
        };

        var lblTitle = new Label
        {
            Text = "🍽️  เลือกร้านอาหาร",
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
        };
        header.Controls.Add(lblTitle);
        this.Controls.Add(header);
    }

    private Panel CreateRestaurantCard(Restaurant restaurant)
    {
        var card = new Panel
        {
            Width = 480,
            Height = 140,
            Margin = new Padding(0, 0, 0, 16),
            BackColor = Color.White,
            Cursor = Cursors.Hand,
        };

        // Arrow button (add before paint so it's created first)
        var btnEnter = new Button
        {
            Text = "เลือก  ›",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = restaurant.BackColor1,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(385, 88),
            Size = new Size(80, 28),
            Cursor = Cursors.Hand,
        };
        btnEnter.FlatAppearance.BorderSize = 0;
        btnEnter.Click += (s, e) => OpenRestaurant(restaurant);

        // Shadow effect via border
        card.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Shadow
            using var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
            g.FillRectangle(shadowBrush, 3, 3, card.Width - 3, card.Height - 3);

            // White card background
            using var cardBrush = new SolidBrush(Color.White);
            var rect = new Rectangle(0, 0, card.Width - 4, card.Height - 4);
            g.FillRectangle(cardBrush, rect);

            // Top color strip
            using var brush1 = new LinearGradientBrush(
                new Point(0, 0), new Point(160, 0),
                restaurant.BackColor1, restaurant.BackColor2);
            g.FillRectangle(brush1, 0, 0, 140, card.Height - 4);

            // Border
            using var pen = new Pen(Color.FromArgb(230, 230, 230));
            g.DrawRectangle(pen, rect);
        };

        // Emoji / name on color strip
        var lblEmoji = new Label
        {
            Text = restaurant.Name.Split(' ')[0],
            Font = new Font("Segoe UI", 32),
            ForeColor = Color.White,
            Location = new Point(10, 30),
            Size = new Size(120, 60),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };

        // Restaurant name
        var lblName = new Label
        {
            Text = string.Join(" ", restaurant.Name.Split(' ').Skip(1)),
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 40, 40),
            Location = new Point(155, 18),
            Size = new Size(300, 28),
            BackColor = Color.Transparent,
        };

        // Description
        var lblDesc = new Label
        {
            Text = restaurant.Description,
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(100, 100, 100),
            Location = new Point(155, 48),
            Size = new Size(300, 20),
            BackColor = Color.Transparent,
        };

        // Tag
        var lblTag = new Label
        {
            Text = "🏷️  " + restaurant.Tag,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(150, 100, 50),
            Location = new Point(155, 72),
            Size = new Size(200, 18),
            BackColor = Color.Transparent,
        };

        // Rating
        var lblRating = new Label
        {
            Text = $"⭐ {restaurant.Rating:0.0}",
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(200, 140, 0),
            Location = new Point(155, 95),
            Size = new Size(90, 20),
            BackColor = Color.Transparent,
        };

        // Delivery time
        var lblTime = new Label
        {
            Text = "🕐 " + restaurant.DeliveryTime,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(80, 140, 80),
            Location = new Point(250, 95),
            Size = new Size(150, 20),
            BackColor = Color.Transparent,
        };

        // Add all non-button controls to card
        card.Controls.Add(lblEmoji);
        card.Controls.Add(lblName);
        card.Controls.Add(lblDesc);
        card.Controls.Add(lblTag);
        card.Controls.Add(lblRating);
        card.Controls.Add(lblTime);

        // Add button last and bring to front
        card.Controls.Add(btnEnter);
        btnEnter.BringToFront();

        // Click whole card
        card.Click += (s, e) => OpenRestaurant(restaurant);
        lblEmoji.Click += (s, e) => OpenRestaurant(restaurant);
        lblName.Click += (s, e) => OpenRestaurant(restaurant);
        lblDesc.Click += (s, e) => OpenRestaurant(restaurant);

        return card;
    }
}

// Model
public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Tag { get; set; } = "";
    public string DeliveryTime { get; set; } = "";
    public float Rating { get; set; }
    public Color BackColor1 { get; set; }
    public Color BackColor2 { get; set; }
    public Dictionary<string, int> Menu { get; set; } = new();
}
