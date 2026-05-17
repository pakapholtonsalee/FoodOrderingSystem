using CustomerApp.Models;

namespace CustomerApp.Data;

/// <summary>
/// Repository Pattern — เก็บและจัดการข้อมูลร้านอาหารทั้งหมด
///
/// ทำไมถึงแยกออกมา?
///   เดิม ShopSelect.cs มีข้อมูลร้านเป็น hardcode อยู่ภายใน
///   ถ้าอยากเปลี่ยนไปดึงจาก API ในอนาคต แก้แค่ไฟล์นี้ไฟล์เดียว
///   ShopSelect ไม่รู้ว่าข้อมูลมาจากไหน — แค่เรียก GetAll() ก็พอ
/// </summary>

public static class RestaurantRepository
{
    /// <summary>
    /// คืนค่ารายชื่อร้านทั้งหมด
    /// IReadOnlyList ป้องกันไม่ให้ผู้เรียก Add/Remove ข้อมูลได้โดยตรง
    /// </summary>
    public static IReadOnlyList<Restaurant> GetAll() => _restaurants;

    // ข้อมูลร้านอาหาร — ตอนนี้ hardcode ไว้ก่อน
    private static readonly Restaurant[] _restaurants =
    {
        new()
        {
            Id           = 1,                         // ต้องตรงกับ Id ใน FoodApi database
            Name         = "🍕 Pizza Palace",
            Description  = "พิซซ่าเตาฟืน อิตาเลียนแท้ๆ",
            Tag          = "Italian · Fastfood",
            DeliveryTime = "25–35 นาที",
            Rating       = 4.8f,
            BackColor1   = Color.FromArgb(76, 175, 80),  // สีหลักของร้าน (เขียว)
            BackColor2   = Color.FromArgb(46, 125, 50),  // สีรองสำหรับ gradient
            Menu = new()
            {
                ["🍕 Margherita Pizza"]  = 199,
                ["🍕 Pepperoni Pizza"]   = 229,
                ["🍕 BBQ Chicken Pizza"] = 249,
                ["🥤 Cola"]              = 39,
                ["🥗 Caesar Salad"]      = 89,
            }
        },
        new()
        {
            Id           = 2,
            Name         = "🍔 Burger Bros",
            Description  = "เบอร์เกอร์เนื้อวากิว พรีเมียม",
            Tag          = "American · Grill",
            DeliveryTime = "20–30 นาที",
            Rating       = 4.6f,
            BackColor1   = Color.FromArgb(198, 81, 21),  // สีหลักของร้าน (ส้มน้ำตาล)
            BackColor2   = Color.FromArgb(150, 50, 10),
            Menu = new()
            {
                ["🍔 Classic Burger"]       = 129,
                ["🍔 Double Cheese Burger"] = 169,
                ["🍔 Wagyu Burger"]         = 299,
                ["🍟 French Fries"]         = 59,
                ["🥤 Milkshake"]            = 89,
            }
        },
    };
}
