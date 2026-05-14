using System.Drawing.Drawing2D;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;

namespace CustomerApp;

public partial class Customer : Form
{
    private readonly Restaurant _restaurant;
    private int _total = 0;
    private readonly List<(string name, int price)> _cartItems = new();

    public Customer(Restaurant restaurant)
    {
        _restaurant = restaurant;
        InitializeComponent();
        SetupUI();
        LoadMenu();
    }

    void SetupUI()
    {
        this.Text = $"🛒 {_restaurant.Name}";
        this.BackColor = Color.FromArgb(255, 248, 240);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Header
        pnlHeader.BackColor = _restaurant.BackColor1;
        lblRestaurantName.Text = _restaurant.Name;
        lblRestaurantName.ForeColor = Color.White;
        lblRestaurantName.Font = new Font("Segoe UI", 16, FontStyle.Bold);

        lblRestaurantDesc.Text = _restaurant.Description + "  •  " + _restaurant.Tag;
        lblRestaurantDesc.ForeColor = Color.FromArgb(255, 220, 180);
        lblRestaurantDesc.Font = new Font("Segoe UI", 9);

        // Section labels
        lblMenuTitle.Text = "📋 เมนู";
        lblMenuTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblMenuTitle.ForeColor = Color.FromArgb(60, 40, 20);

        lblCartTitle.Text = "🛒 ตะกร้า";
        lblCartTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblCartTitle.ForeColor = Color.FromArgb(60, 40, 20);

        // Buttons
        StyleButton(btnAdd, "➕ เพิ่ม", Color.FromArgb(76, 175, 80));
        StyleButton(btnRemove, "✖ ลบ", Color.FromArgb(229, 57, 53));
        StyleButton(btnOrder, "📦 สั่งอาหาร", _restaurant.BackColor1);
        StyleButton(btnStatus, "📊 ดูสถานะ", Color.FromArgb(63, 81, 181));
        StyleButton(btnBack, "← กลับ", Color.FromArgb(150, 150, 150));

        lblTotal.Font = new Font("Segoe UI", 13, FontStyle.Bold);
        lblTotal.ForeColor = Color.FromArgb(200, 60, 20);
        lblTotal.Text = "รวม: ฿0";

        // ListBoxes
        StyleListBox(listBoxMenu);
        StyleListBox(listBoxCart);
    }

    void StyleButton(Button btn, string text, Color color)
    {
        btn.Text = text;
        btn.BackColor = color;
        btn.ForeColor = Color.White;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
        btn.Cursor = Cursors.Hand;
        btn.Height = 45;
        btn.Padding = new Padding(8, 6, 8, 6);
    }

    void StyleListBox(ListBox lb)
    {
        lb.Font = new Font("Segoe UI", 10);
        lb.BorderStyle = BorderStyle.FixedSingle;
        lb.BackColor = Color.White;
        lb.DrawMode = DrawMode.OwnerDrawFixed;
        lb.ItemHeight = 38;
        lb.DrawItem += ListBox_DrawItem;
    }

    private void ListBox_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;
        var lb = (ListBox)sender!;
        e.DrawBackground();

        var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        var bgColor = isSelected ? Color.FromArgb(255, 235, 210) : (e.Index % 2 == 0 ? Color.White : Color.FromArgb(252, 252, 252));
        e.Graphics.FillRectangle(new SolidBrush(bgColor), e.Bounds);

        if (isSelected)
        {
            using var selPen = new Pen(_restaurant.BackColor1, 2);
            e.Graphics.DrawRectangle(selPen, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1));
        }

        var text = lb.Items[e.Index].ToString() ?? "";
        using var brush = new SolidBrush(Color.FromArgb(40, 40, 40));
        e.Graphics.DrawString(text, lb.Font, brush, e.Bounds.X + 10, e.Bounds.Y + 10);

        e.DrawFocusRectangle();
    }

    void LoadMenu()
    {
        listBoxMenu.Items.Clear();
        foreach (var (name, price) in _restaurant.Menu)
        {
            listBoxMenu.Items.Add($"{name}  —  ฿{price}");
        }
    }

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        if (listBoxMenu.SelectedItem == null) return;

        var selected = listBoxMenu.SelectedItem.ToString()!;
        // Parse name and price from "Name — ฿price"
        var parts = selected.Split("  —  ฿");
        var itemName = parts[0].Trim();
        var itemPrice = int.Parse(parts[1].Trim());

        _cartItems.Add((itemName, itemPrice));
        listBoxCart.Items.Add($"{itemName}  —  ฿{itemPrice}");

        _total += itemPrice;
        lblTotal.Text = $"รวม: ฿{_total:N0}";
    }

    private void btnRemove_Click(object? sender, EventArgs e)
    {
        if (listBoxCart.SelectedIndex < 0) return;
        var idx = listBoxCart.SelectedIndex;
        _total -= _cartItems[idx].price;
        _cartItems.RemoveAt(idx);
        listBoxCart.Items.RemoveAt(idx);
        lblTotal.Text = $"รวม: ฿{_total:N0}";
    }

    private async void btnOrder_Click(object? sender, EventArgs e)
    {
        if (_cartItems.Count == 0)
        {
            MessageBox.Show("กรุณาเพิ่มสินค้าในตะกร้าก่อนสั่ง", "แจ้งเตือน",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnOrder.Enabled = false;
        btnOrder.Text = "⏳ กำลังสั่ง...";

        var order = new
        {
            customerId = 1,           // ← ใส่ id จริงของ user ที่ login (ตอนนี้ hardcode ไว้ก่อน)
            restaurantId = _restaurant.Id,
            status = "Pending",
            items = _cartItems.Select(x => new   // ← เปลี่ยนจาก .Select(x => x.name)
            {
                foodName = x.name,
                quantity = 1,
                price = (decimal)x.price
            }).ToList()
            // ลบ total ออก เพราะ API คำนวณเองแล้ว
        };

        var json = JsonSerializer.Serialize(order);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var client = new HttpClient();
            var response = await client.PostAsync("https://localhost:7172/api/orders", content);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                var resultDoc = JsonDocument.Parse(resultJson);
                var orderId = resultDoc.RootElement.GetProperty("id").GetInt32();

                _cartItems.Clear();
                listBoxCart.Items.Clear();
                _total = 0;
                lblTotal.Text = "รวม: ฿0";

                // await Task.Delay(1500);

                MessageBox.Show($"✅ สั่งอาหารสำเร็จ!\nออเดอร์ #{orderId}", "สำเร็จ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                var statusForm = new OrderStatus(orderId, _restaurant.Name);
                statusForm.Show();
            }
            else
            {
                MessageBox.Show("❌ สั่งอาหารไม่สำเร็จ กรุณาลองใหม่", "ผิดพลาด",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"❌ เชื่อมต่อ Server ไม่ได้\n{ex.Message}", "ผิดพลาด",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnOrder.Enabled = true;
            btnOrder.Text = "📦 สั่งอาหาร";
        }
    }

    private void btnStatus_Click(object? sender, EventArgs e)
    {
        var statusForm = new OrderStatus(0, _restaurant.Name);
        statusForm.Show();
    }

    private void btnBack_Click(object? sender, EventArgs e)
    {
        this.Close();
    }
}
