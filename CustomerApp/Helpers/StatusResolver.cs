using CustomerApp.Models;

namespace CustomerApp.Helpers;

/// <summary>
/// Strategy Pattern — แปลง status string เป็น StatusInfo ที่พร้อมแสดงผล
///
/// ทำไมถึงแยกออกมา?
///   เดิม OrderStatus.cs มี GetStatusInfo() ที่ map status → color/emoji
///   และมีการ hardcode ซ้ำอีกรอบใน BuildLegend()
///   ตอนนี้: แก้ที่นี่ที่เดียว ทั้ง badge, progress bar, และ legend ได้สีเดียวกันเสมอ
///
/// การใช้งาน:
///   var info = StatusResolver.Resolve("delivering");
///   // info.Emoji = "🛵", info.Label = "กำลังเดินทางจัดส่ง", info.Color = Color.Green
/// </summary>
public static class StatusResolver
{
    // Dictionary map status string → StatusInfo
    // StringComparer.OrdinalIgnoreCase ทำให้ "Preparing" และ "preparing" ให้ผลเหมือนกัน
    private static readonly Dictionary<string, StatusInfo> _map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["preparing"] = new("👨‍🍳", "กำลังจัดเตรียม", Color.FromArgb(33, 150, 243)),
            ["ready"] = new("✅", "ร้านทำเสร็จแล้ว", Color.FromArgb(76, 175, 80)),
            ["delivering"] = new("🛵", "กำลังเดินทางจัดส่งอาหาร", Color.FromArgb(46, 125, 50)),
            // API uses "PickedUp" when rider picks the order — treat it same as delivering
            ["pickedup"]   = new("🛵", "กำลังเดินทางจัดส่งอาหาร", Color.FromArgb(46, 125, 50)),
            ["delivered"] = new("🎉", "จัดส่งสำเร็จ", Color.FromArgb(27, 94, 32)),
        };

    // ถ้า status ไม่ match กับ key ใดเลย (เช่น "Pending" หรือ null) → ใช้ค่า default นี้
    private static readonly StatusInfo _default =
        new("⏳", "รอรับออเดอร์", Color.FromArgb(255, 152, 0));

    /// <summary>
    /// รับ status string จาก API แล้วคืน StatusInfo สำหรับแสดงผล
    /// ถ้าไม่ match → คืน _default (สีส้ม "รอรับออเดอร์")
    /// </summary>
    public static StatusInfo Resolve(string? status) =>
        status != null && _map.TryGetValue(status, out var info) ? info : _default;

    /// <summary>
    /// คืนค่า % ความคืบหน้าสำหรับ progress bar
    /// ใช้ใน CreateOrderCard() ของ OrderStatus เพื่อวาด progress bar ใต้แต่ละ card
    /// ถ้าเป็น Pending (0%) จะไม่สร้าง progress bar เลย
    /// </summary>

    public static int GetProgress(string? status) => status?.ToLower() switch
    {
        "preparing" => 50,   // กำลังทำ — ครึ่งทาง
        "ready" => 75,   // ทำเสร็จแล้ว รอไรเดอร์
        "pickedup"  => 90,
        "delivered" => 100,  // ส่งถึงแล้ว — เต็ม
        _ => 0,    // Pending หรือไม่รู้จัก — ไม่แสดง bar
    };
}
