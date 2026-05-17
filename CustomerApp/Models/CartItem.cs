namespace CustomerApp.Models;

/// <summary>
/// แทนรายการอาหาร 1 ชิ้นที่อยู่ในตะกร้า
///
/// ใช้ record แทน class เพราะ:
///   - เป็น immutable (สร้างแล้วเปลี่ยนค่าไม่ได้) — ป้องกัน bug จากการแก้ item ที่อยู่ใน cart โดยไม่ตั้งใจ
///   - มี value equality ในตัว (record เปรียบเทียบค่า ไม่ใช่ reference)
///   - syntax กระชับกว่า class
/// </summary>
public record CartItem(string Name, int Price)
{
    // ใช้แสดงใน ListBox ของตะกร้า เช่น "🍕 Margherita Pizza  —  ฿199"
    public override string ToString() => $"{Name}  —  ฿{Price}";
}
