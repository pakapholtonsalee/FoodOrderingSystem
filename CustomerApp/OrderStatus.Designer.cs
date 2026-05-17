namespace CustomerApp
{
    partial class OrderStatus
    {
        // คอนเทนเนอร์สำหรับเก็บ component ที่สร้างโดย Windows Forms Designer
        // ใช้เพื่อเรียก Dispose ของ component เหล่านี้เมื่อฟอร์มถูกทิ้ง
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// ปลดปล่อยทรัพยากรที่ฟอร์มใช้งาน
        /// disposing == true : ปลดปล่อย managed resources (เช่น components)
        /// เรียก base.Dispose เพื่อให้ .NET ดูแล unmanaged resources ต่อ
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// เมทอดที่สร้างโดย Windows Forms Designer
        /// ตั้งค่าพื้นฐานของฟอร์ม ได้แก่ การสเกล, ขนาดเริ่มต้น, ชื่อ และข้อความหัวข้อ
        /// ห้ามลบหรือเปลี่ยนชื่อเมทอดนี้ถ้าต้องการใช้ Designer ต่อ
        /// </summary>
        private void InitializeComponent()
        {
            // หยุดการจัดวาง UI ชั่วคราวก่อนตั้งค่าคอนโทรลเพื่อประสิทธิภาพ
            this.SuspendLayout();

            // ขนาดมาตรฐานสำหรับ AutoScale (ใช้สำหรับการปรับขนาดฟอนต์และคอนโทรล)
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            // ขนาดหน้าต่างเริ่มต้น (Client area)
            this.ClientSize = new System.Drawing.Size(620, 580);

            // ชื่อโค้ดของฟอร์มและข้อความที่แสดงบน title bar
            this.Name = "OrderStatus";
            this.Text = "📊 สถานะออเดอร์";

            // คืนค่าการจัดวางและแสดงคอนโทรลที่เพิ่มเข้ามา
            this.ResumeLayout(false);
        }
    }
}
