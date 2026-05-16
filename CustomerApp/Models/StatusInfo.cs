namespace CustomerApp.Models;

/// <summary>
/// Value Object เก็บข้อมูลที่ใช้แสดงสถานะออเดอร์
///
/// ถูกสร้างโดย StatusResolver.Resolve() แล้วนำไปใช้ใน OrderStatus เพื่อ:
///   - แสดง Emoji + Label บน status badge ของแต่ละ card
///   - กำหนดสี (Color) ของ badge, progress bar, และ legend
///
/// ใช้ record เพราะเป็นข้อมูลที่ไม่ต้องเปลี่ยนค่า (immutable)
/// </summary>
public record StatusInfo(string Emoji, string Label, Color Color);
//                        ↑ เช่น "🛵"  ↑ "กำลังจัดส่ง"  ↑ Color.Green
