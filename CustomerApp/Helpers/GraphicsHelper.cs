using System.Drawing.Drawing2D;

namespace CustomerApp.Helpers;

/// <summary>
/// Helper สำหรับ custom drawing ที่ใช้ซ้ำหลายที่
/// ตอนนี้มีแค่ RoundedRect ซึ่งใช้วาด status badge ใน OrderStatus
/// </summary>
public static class GraphicsHelper
{
    /// <summary>
    /// สร้าง GraphicsPath รูปสี่เหลี่ยมมุมมน
    /// WinForms ไม่มี built-in rounded rect สำหรับ custom drawing
    /// จึงต้องสร้าง Path เองโดยต่อ arc 4 มุมเข้าหากัน

    /// ใช้ใน CreateStatusBadge() ของ OrderStatus เพื่อวาดกรอบ badge มุมมน
    /// </summary>
    /// <param name="rect">ขนาดและตำแหน่งของสี่เหลี่ยม</param>
    /// <param name="radius">ความโค้งของมุม (ยิ่งมาก ยิ่งโค้ง)</param>
    public static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();

        // วาด arc ทีละมุม ตามเข็มนาฬิกา เริ่มจากมุมบนซ้าย
        // AddArc parameters: (x, y, width, height, startAngle, sweepAngle)
        path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90); // มุมบนซ้าย
        path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90); // มุมบนขวา
        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90); // มุมล่างขวา
        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90); // มุมล่างซ้าย

        path.CloseFigure(); // ต่อเส้นจากจุดสุดท้ายกลับไปจุดแรก ทำให้ path ปิดสมบูรณ์
        return path;
    }
}
