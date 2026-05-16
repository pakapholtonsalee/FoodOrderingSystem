namespace CustomerApp
{
    partial class ShopSelect
    {
        // คอนเทนเนอร์สำหรับเก็บ component ที่สร้างโดย designer
        // ถูกใช้งานเพื่อให้สามารถเรียก Dispose ของ component เหล่านี้ได้เมื่อฟอร์มถูกทำลาย
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// ทำความสะอาดทรัพยากรที่ถูกใช้โดยฟอร์ม
        /// disposing == true : ให้ปลดปล่อย managed resources (เช่น components)
        /// เรียก base.Dispose เพื่อให้ .NET ปลดปล่อย unmanaged resources ต่อ
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// เมทอดที่ถูกสร้างโดย Windows Forms Designer
        /// ตั้งค่าพื้นฐานของฟอร์ม เช่น ขนาดเริ่มต้น, โหมดการสเกล และชื่อฟอร์ม
        /// ห้ามลบหรือเปลี่ยนชื่อเมทอดนี้ถ้าใช้ designer ต่อไป
        /// </summary>
        private void InitializeComponent()
        {
            // หยุดการจัดวางชั่วคราวก่อนตั้งค่าคอนโทรลเพื่อเพิ่มประสิทธิภาพ
            this.SuspendLayout();

            // ขนาดมาตรฐานสำหรับการสเกลของฟอนต์และคอนโทรล
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            // ขนาดหน้าต่างเริ่มต้นและขนาดต่ำสุด
            this.ClientSize = new System.Drawing.Size(560, 480);
            this.MinimumSize = new System.Drawing.Size(560, 480);

            // ชื่อและข้อความหัวข้อของฟอร์ม (จะถูกตั้งใหม่ในโค้ดหลัก แต่ designer เก็บค่าเบื้องต้น)
            this.Name = "ShopSelect";
            this.Text = "เลือกร้านอาหาร";

            // คืนค่าการจัดวางและแสดงคอนโทรลที่ถูกเพิ่ม
            this.ResumeLayout(false);
        }
    }
}
