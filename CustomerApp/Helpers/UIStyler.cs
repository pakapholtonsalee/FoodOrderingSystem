namespace CustomerApp.Helpers;

/// <summary>
/// Factory Method Pattern — สร้าง UI Controls พร้อม style สำเร็จรูป
///
/// ทำไมถึงมีไฟล์นี้?
///   เดิม Customer.cs และ OrderStatus.cs มีโค้ด styling Button และ Panel ซ้ำกัน
///   แทนที่จะ copy-paste และเสี่ยง style ไม่ consistent → รวมไว้ที่นี่ที่เดียว
///   ถ้าอยากเปลี่ยนสี font หรือ padding ของทุก button → แก้ที่นี่จุดเดียวพอ
/// </summary>
public static class UIStyler
{
    /// <summary>
    /// สร้าง Button ที่มี style มาตรฐาน (พื้นสีทึบ ตัวอักษรขาว ไม่มี border)
    /// ใช้ใน Customer form (btnAdd, btnRemove, btnOrder, ฯลฯ) และ OrderStatus (btnRefresh)
    /// </summary>
    public static Button CreateStyledButton(string text, Color backColor)
    {
        return new Button
        {
            Text = text,
            BackColor = backColor,      // สีพื้นหลังตามที่ส่งมา เช่น สีเขียวสำหรับปุ่ม "เพิ่ม"
            ForeColor = Color.White,    // ตัวอักษรสีขาวเสมอ
            FlatStyle = FlatStyle.Flat, // ไม่มี 3D effect
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            Cursor = Cursors.Hand,   // cursor เปลี่ยนเป็นมือเมื่อ hover
            Height = 45,
            Padding = new Padding(8, 6, 8, 6),
            FlatAppearance = { BorderSize = 0 }, // ไม่มีกรอบ
        };
    }

    /// <summary>
    /// สร้าง Panel header พร้อม Label ชื่อหน้า
    /// ใช้ใน ShopSelect ("🍽️ เลือกร้านอาหาร") และ OrderStatus ("📊 สถานะออเดอร์")
    /// Dock = Top ทำให้ติดด้านบน Form เสมอ
    /// </summary>
    public static Panel CreateHeaderPanel(Color backColor, int height, string title, float fontSize = 18)
    {
        var header = new Panel { Dock = DockStyle.Top, Height = height, BackColor = backColor };
        header.Controls.Add(new Label
        {
            Text = title,
            Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,          // Label ขยายเต็ม Panel
            TextAlign = ContentAlignment.MiddleCenter, // ข้อความอยู่กลาง
        });
        return header;
    }
}
