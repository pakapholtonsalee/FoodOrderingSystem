namespace CustomerApp
{
    internal static class Program
    {
        [STAThread] // บอกว่า Thread หลักของ WinForms ต้องเป็น Single-Threaded Apartment (ข้อกำหนดของ Windows UI)
        static void Main()
        {
            ApplicationConfiguration.Initialize(); // ตั้งค่าพื้นฐานของ WinForms เช่น DPI awareness, visual styles

            // เปิด Form แรกคือ ShopSelect แล้วให้ Application วนรอ event จาก user
            // เมื่อ ShopSelect ปิด → Application จะจบการทำงาน
            Application.Run(new ShopSelect());
        }
    }
}
