using CustomerApp.Models;

namespace CustomerApp.Services;

/// <summary>
/// จัดการ state ของตะกร้าสินค้าทั้งหมด
///
/// Observer Pattern:
///   CartService เป็น "Publisher" — เมื่อตะกร้าเปลี่ยน จะยิง event CartChanged
///   Customer form เป็น "Subscriber" — subscribe event แล้วอัปเดต UI อัตโนมัติ
///   ทำให้ไม่ต้องเรียก RefreshCartDisplay() เองทุกครั้งที่แก้ตะกร้า
///
/// Single Responsibility:
///   ทุก logic เกี่ยวกับตะกร้าอยู่ที่นี่ที่เดียว ไม่กระจายใน Form
/// </summary>
public class CartService
{
    // เก็บรายการใน List ภายใน ไม่ให้ข้างนอกแก้ไขตรงๆ ได้
    private readonly List<CartItem> _items = new();

    // คืน List แบบ read-only — ข้างนอกดูได้ แต่ Add/Remove ผ่านตรงนี้ไม่ได้
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    // คำนวณยอดรวม ณ ตอนนั้น — Customer form ใช้แสดงใน lblTotal
    public int Total => _items.Sum(i => i.Price);

    // ใช้ตรวจก่อนกด "สั่งอาหาร" — ถ้า true จะแสดง warning แทน
    public bool IsEmpty => _items.Count == 0;

    /// <summary>
    /// Event ที่ยิงออกทุกครั้งที่ตะกร้าเปลี่ยน (เพิ่ม/ลบ/ล้าง)
    /// Customer form subscribe ไว้: _cart.CartChanged += RefreshCartDisplay;
    /// </summary>
    public event Action? CartChanged;

    /// <summary>เพิ่มรายการลงตะกร้า แล้วแจ้ง subscriber</summary>
    public void Add(CartItem item)
    {
        _items.Add(item);
        CartChanged?.Invoke(); // ?.Invoke() = เรียกเฉพาะถ้ามี subscriber อยู่
    }

    /// <summary>ลบรายการตาม index แล้วแจ้ง subscriber</summary>
    public void RemoveAt(int index)
    {
        // ป้องกัน IndexOutOfRangeException ก่อนลบ
        if (index < 0 || index >= _items.Count) return;
        _items.RemoveAt(index);
        CartChanged?.Invoke();
    }

    /// <summary>ล้างตะกร้าทั้งหมด — เรียกหลังสั่งอาหารสำเร็จ</summary>
    public void Clear()
    {
        _items.Clear();
        CartChanged?.Invoke();
    }
}
