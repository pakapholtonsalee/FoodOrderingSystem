namespace CustomerApp
{
    partial class Customer
    {
        // คอนเทนเนอร์ที่เก็บ component ที่สร้างโดย Designer
        // ใช้เพื่อติดตามและปลดปล่อย managed resources เมื่อฟอร์มถูกทำลาย
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
        /// เมทอดที่ Windows Forms Designer สร้างไว้
        /// ใช้ตั้งค่าคอนโทรลบนฟอร์มทั้งหมด (header, body, bottom, ปุ่ม และ listbox)
        /// ข้อสังเกต: เมทอดนี้มักถูกแก้ไขโดย designer GUI — ระวังเมื่อต้องแก้โค้ดด้วยมือ
        /// </summary>
        private void InitializeComponent()
        {
            // สร้าง instance ของคอนโทรลที่ต้องการใช้บนฟอร์ม
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblRestaurantName = new System.Windows.Forms.Label();
            this.lblRestaurantDesc = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnOrder = new System.Windows.Forms.Button();
            this.btnStatus = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.lblCartTitle = new System.Windows.Forms.Label();
            this.listBoxCart = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblMenuTitle = new System.Windows.Forms.Label();
            this.listBoxMenu = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();

            // หยุดการจัดวางชั่วคราวเพื่อเพิ่มประสิทธิภาพขณะตั้งค่าคอนโทรล
            this.pnlHeader.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.SuspendLayout();

            // --- Header panel: แสดงชื่อและคำอธิบายร้าน ---
            this.pnlHeader.Controls.Add(this.lblRestaurantDesc);
            this.pnlHeader.Controls.Add(this.lblRestaurantName);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top; // จัดชิดด้านบน
            this.pnlHeader.Height = 80;
            this.pnlHeader.Name = "pnlHeader";

            // ชื่อร้าน (แสดงเป็นข้อความขนาดใหญ่)
            this.lblRestaurantName.AutoSize = false;
            this.lblRestaurantName.Location = new System.Drawing.Point(16, 10);
            this.lblRestaurantName.Name = "lblRestaurantName";
            this.lblRestaurantName.Size = new System.Drawing.Size(600, 32);
            this.lblRestaurantName.Text = ""; // ถูกกำหนดค่าจริงจากโค้ดที่สร้างฟอร์ม

            // คำอธิบายร้าน (บรรทัดรอง)
            this.lblRestaurantDesc.AutoSize = false;
            this.lblRestaurantDesc.Location = new System.Drawing.Point(16, 46);
            this.lblRestaurantDesc.Name = "lblRestaurantDesc";
            this.lblRestaurantDesc.Size = new System.Drawing.Size(600, 22);
            this.lblRestaurantDesc.Text = "";

            // --- Bottom panel: แสดงยอดรวมและปุ่มคำสั่ง (กลับ, ดูสถานะ, สั่งอาหาร) ---
            this.pnlBottom.Controls.Add(this.btnBack);
            this.pnlBottom.Controls.Add(this.btnStatus);
            this.pnlBottom.Controls.Add(this.btnOrder);
            this.pnlBottom.Controls.Add(this.lblTotal);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom; // จัดชิดด้านล่าง
            this.pnlBottom.Height = 70;
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(255, 240, 220);
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);

            // Label แสดงยอดรวมราคาปัจจุบันของตะกร้า
            this.lblTotal.AutoSize = false;
            this.lblTotal.Location = new System.Drawing.Point(180, 18);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(160, 34);
            this.lblTotal.Text = "รวม: ฿0"; // ค่าเริ่มต้น
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ปุ่มย้อนกลับไปยังหน้าร้าน (ShopSelect)
            this.btnBack.Location = new System.Drawing.Point(16, 17);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(90, 36);
            this.btnBack.Text = "← กลับ";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            // ปุ่มเปิดหน้าดูสถานะออเดอร์
            this.btnStatus.Location = new System.Drawing.Point(360, 17);
            this.btnStatus.Name = "btnStatus";
            this.btnStatus.Size = new System.Drawing.Size(130, 36);
            this.btnStatus.Text = "📊 ดูสถานะ";
            this.btnStatus.Click += new System.EventHandler(this.btnStatus_Click);

            // ปุ่มสั่งอาหารจากตะกร้า
            this.btnOrder.Location = new System.Drawing.Point(500, 17);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(140, 36);
            this.btnOrder.Text = "📦 สั่งอาหาร";
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);

            // --- Body panel: แบ่งเป็นซ้าย (เมนู) และขวา (ตะกร้า) ---
            this.pnlBody.Controls.Add(this.pnlRight);
            this.pnlBody.Controls.Add(this.pnlLeft);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill; // เติมพื้นที่กลางของฟอร์ม
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);

            // Left: เมนูรายการอาหาร (listBoxMenu) และปุ่มเพิ่ม
            this.pnlLeft.Controls.Add(this.btnAdd);
            this.pnlLeft.Controls.Add(this.listBoxMenu);
            this.pnlLeft.Controls.Add(this.lblMenuTitle);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Width = 300;

            // หัวข้อเมนู
            this.lblMenuTitle.AutoSize = false;
            this.lblMenuTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMenuTitle.Height = 32;
            this.lblMenuTitle.Name = "lblMenuTitle";
            this.lblMenuTitle.Text = "📋 เมนู";
            this.lblMenuTitle.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);

            // รายการเมนู (แสดงรายการอาหารให้กดเลือกเพื่อเพิ่มลงตะกร้า)
            this.listBoxMenu.Name = "listBoxMenu";
            this.listBoxMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxMenu.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.listBoxMenu.TabIndex = 0;

            // ปุ่มเพิ่มเมนูที่เลือกลงตะกร้า
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Text = "➕ เพิ่ม";
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAdd.Height = 45;
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // Right: ตะกร้าสินค้า (listBoxCart) และปุ่มลบ
            this.pnlRight.Controls.Add(this.btnRemove);
            this.pnlRight.Controls.Add(this.listBoxCart);
            this.pnlRight.Controls.Add(this.lblCartTitle);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);

            // หัวข้อของตะกร้า
            this.lblCartTitle.AutoSize = false;
            this.lblCartTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCartTitle.Height = 32;
            this.lblCartTitle.Name = "lblCartTitle";
            this.lblCartTitle.Text = "🛒 ตะกร้า";
            this.lblCartTitle.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);

            // รายการสินค้าในตะกร้า
            this.listBoxCart.Name = "listBoxCart";
            this.listBoxCart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxCart.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.listBoxCart.TabIndex = 2;

            // ปุ่มลบรายการที่เลือกในตะกร้า
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Text = "✖ ลบ";
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRemove.Height = 45;
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);

            // --- ตั้งค่าฟอร์มหลัก: ขนาดเริ่มต้น และเพิ่ม panel ที่สร้างไว้ ---
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 480);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(680, 480);
            this.Name = "Customer";
            this.Text = "Customer";

            // คืนค่าการจัดวางที่ระงับไว้และแสดงคอนโทรล
            this.pnlHeader.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ประกาศฟิลด์ของคอนโทรลทั้งหมดที่ใช้ในฟอร์ม
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblRestaurantName;
        private System.Windows.Forms.Label lblRestaurantDesc;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Button btnStatus;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel pnlBody;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Label lblMenuTitle;
        private System.Windows.Forms.ListBox listBoxMenu;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblCartTitle;
        private System.Windows.Forms.ListBox listBoxCart;
        private System.Windows.Forms.Button btnRemove;
    }
}