using CustomerApp.Models;
using CustomerApp.Services;

namespace CustomerApp;

/// <summary>
/// หน้าสั่งอาหาร — แสดงเมนูและตะกร้าสินค้า
///
/// Design Patterns:
///   • Dependency Injection : รับ CartService และ IOrderService ผ่าน constructor
///   • Observer             : CartService.CartChanged → RefreshCartDisplay() อัตโนมัติ
/// </summary>
public partial class Customer : Form
{
    private readonly Restaurant    _restaurant;
    private readonly CartService   _cart;
    private readonly IOrderService _orderService;

    public Customer(Restaurant restaurant, CartService cart, IOrderService orderService)
    {
        _restaurant   = restaurant;
        _cart         = cart;
        _orderService = orderService;

        InitializeComponent();
        SetupUI();
        LoadMenu();

        // Observer: เมื่อตะกร้าเปลี่ยน UI อัปเดตยอดรวมอัตโนมัติ
        _cart.CartChanged += RefreshCartDisplay;
    }

    // ── UI Setup ──────────────────────────────────────────────────────

    private void SetupUI()
    {
        this.Text          = $"🛒 {_restaurant.Name}";
        this.BackColor     = Color.FromArgb(255, 248, 240);
        this.StartPosition = FormStartPosition.CenterScreen;

        pnlHeader.BackColor         = _restaurant.BackColor1;
        lblRestaurantName.Text      = _restaurant.Name;
        lblRestaurantName.ForeColor = Color.White;
        lblRestaurantName.Font      = new Font("Segoe UI", 16, FontStyle.Bold);

        lblRestaurantDesc.Text      = $"{_restaurant.Description}  •  {_restaurant.Tag}";
        lblRestaurantDesc.ForeColor = Color.FromArgb(255, 220, 180);
        lblRestaurantDesc.Font      = new Font("Segoe UI", 9);

        lblMenuTitle.Text      = "📋 เมนู";
        lblMenuTitle.Font      = new Font("Segoe UI", 11, FontStyle.Bold);
        lblMenuTitle.ForeColor = Color.FromArgb(60, 40, 20);

        lblCartTitle.Text      = "🛒 ตะกร้า";
        lblCartTitle.Font      = new Font("Segoe UI", 11, FontStyle.Bold);
        lblCartTitle.ForeColor = Color.FromArgb(60, 40, 20);

        StyleButton(btnAdd,    "➕ เพิ่ม",      Color.FromArgb(76, 175, 80));
        StyleButton(btnRemove, "✖ ลบ",          Color.FromArgb(229, 57, 53));
        StyleButton(btnOrder,  "📦 สั่งอาหาร", _restaurant.BackColor1);
        StyleButton(btnStatus, "📊 ดูสถานะ",   Color.FromArgb(63, 81, 181));
        StyleButton(btnBack,   "← กลับ",        Color.FromArgb(150, 150, 150));

        lblTotal.Font      = new Font("Segoe UI", 13, FontStyle.Bold);
        lblTotal.ForeColor = Color.FromArgb(200, 60, 20);
        lblTotal.Text      = "รวม: ฿0";

        StyleListBox(listBoxMenu);
        StyleListBox(listBoxCart);
    }

    private static void StyleButton(Button btn, string text, Color color)
    {
        btn.Text                      = text;
        btn.BackColor                 = color;
        btn.ForeColor                 = Color.White;
        btn.FlatStyle                 = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font                      = new Font("Segoe UI", 11f, FontStyle.Bold);
        btn.Cursor                    = Cursors.Hand;
        btn.Height                    = 45;
        btn.Padding                   = new Padding(8, 6, 8, 6);
    }

    private void StyleListBox(ListBox lb)
    {
        lb.Font        = new Font("Segoe UI", 10);
        lb.BorderStyle = BorderStyle.FixedSingle;
        lb.BackColor   = Color.White;
        lb.DrawMode    = DrawMode.OwnerDrawFixed;
        lb.ItemHeight  = 38;
        lb.DrawItem   += ListBox_DrawItem;
    }

    private void ListBox_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || sender is not ListBox lb) return;

        e.DrawBackground();

        var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        var bgColor    = isSelected
            ? Color.FromArgb(255, 235, 210)
            : (e.Index % 2 == 0 ? Color.White : Color.FromArgb(252, 252, 252));

        e.Graphics.FillRectangle(new SolidBrush(bgColor), e.Bounds);

        if (isSelected)
        {
            using var pen = new Pen(_restaurant.BackColor1, 2);
            e.Graphics.DrawRectangle(pen,
                new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1));
        }

        var text = lb.Items[e.Index]?.ToString() ?? "";
        using var brush = new SolidBrush(Color.FromArgb(40, 40, 40));
        e.Graphics.DrawString(text, lb.Font, brush, e.Bounds.X + 10, e.Bounds.Y + 10);

        e.DrawFocusRectangle();
    }

    private void LoadMenu()
    {
        listBoxMenu.Items.Clear();
        foreach (var (name, price) in _restaurant.Menu)
            listBoxMenu.Items.Add($"{name}  —  ฿{price}");
    }

    // ── Event Handlers ────────────────────────────────────────────────

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        if (listBoxMenu.SelectedItem is not string selected) return;

        var parts    = selected.Split("  —  ฿");
        var itemName  = parts[0].Trim();
        var itemPrice = int.Parse(parts[1].Trim());

        var item = new CartItem(itemName, itemPrice);
        _cart.Add(item);
        listBoxCart.Items.Add(item.ToString());
    }

    private void btnRemove_Click(object? sender, EventArgs e)
    {
        var idx = listBoxCart.SelectedIndex;
        if (idx < 0) return;

        _cart.RemoveAt(idx);
        listBoxCart.Items.RemoveAt(idx);
    }

    private async void btnOrder_Click(object? sender, EventArgs e)
    {
        if (_cart.IsEmpty)
        {
            MessageBox.Show("กรุณาเพิ่มสินค้าในตะกร้าก่อนสั่ง", "แจ้งเตือน",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SetOrderingState(true);

        try
        {
            var orderId = await _orderService.PlaceOrderAsync(_restaurant.Id, _cart.Items);

            _cart.Clear();
            listBoxCart.Items.Clear();

            MessageBox.Show($"✅ สั่งอาหารสำเร็จ!\nออเดอร์ #{orderId}", "สำเร็จ",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            new OrderStatus(orderId, _restaurant.Name, _orderService).Show();
        }
        catch (HttpRequestException)
        {
            MessageBox.Show("❌ สั่งอาหารไม่สำเร็จ กรุณาลองใหม่", "ผิดพลาด",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"❌ เชื่อมต่อ Server ไม่ได้\n{ex.Message}", "ผิดพลาด",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetOrderingState(false);
        }
    }

    private void btnStatus_Click(object? sender, EventArgs e) =>
        new OrderStatus(0, _restaurant.Name, _orderService).Show();

    private void btnBack_Click(object? sender, EventArgs e) => this.Close();

    // ── Helpers ───────────────────────────────────────────────────────

    /// <summary>Observer callback จาก CartService</summary>
    private void RefreshCartDisplay() =>
        lblTotal.Text = $"รวม: ฿{_cart.Total:N0}";

    private void SetOrderingState(bool isOrdering)
    {
        btnOrder.Enabled = !isOrdering;
        btnOrder.Text    = isOrdering ? "⏳ กำลังสั่ง..." : "📦 สั่งอาหาร";
    }
}
