namespace CustomerApp
{
    partial class Customer
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
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

            this.pnlHeader.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.SuspendLayout();

            // pnlHeader
            this.pnlHeader.Controls.Add(this.lblRestaurantDesc);
            this.pnlHeader.Controls.Add(this.lblRestaurantName);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height = 80;
            this.pnlHeader.Name = "pnlHeader";

            // lblRestaurantName
            this.lblRestaurantName.AutoSize = false;
            this.lblRestaurantName.Location = new System.Drawing.Point(16, 10);
            this.lblRestaurantName.Name = "lblRestaurantName";
            this.lblRestaurantName.Size = new System.Drawing.Size(600, 32);
            this.lblRestaurantName.Text = "";

            // lblRestaurantDesc
            this.lblRestaurantDesc.AutoSize = false;
            this.lblRestaurantDesc.Location = new System.Drawing.Point(16, 46);
            this.lblRestaurantDesc.Name = "lblRestaurantDesc";
            this.lblRestaurantDesc.Size = new System.Drawing.Size(600, 22);
            this.lblRestaurantDesc.Text = "";

            // pnlBottom
            this.pnlBottom.Controls.Add(this.btnBack);
            this.pnlBottom.Controls.Add(this.btnStatus);
            this.pnlBottom.Controls.Add(this.btnOrder);
            this.pnlBottom.Controls.Add(this.lblTotal);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Height = 70;
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(255, 240, 220);
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);

            // lblTotal
            this.lblTotal.AutoSize = false;
            this.lblTotal.Location = new System.Drawing.Point(180, 18);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(160, 34);
            this.lblTotal.Text = "รวม: ฿0";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // btnBack
            this.btnBack.Location = new System.Drawing.Point(16, 17);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(90, 36);
            this.btnBack.Text = "← กลับ";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            // btnStatus
            this.btnStatus.Location = new System.Drawing.Point(360, 17);
            this.btnStatus.Name = "btnStatus";
            this.btnStatus.Size = new System.Drawing.Size(130, 36);
            this.btnStatus.Text = "📊 ดูสถานะ";
            this.btnStatus.Click += new System.EventHandler(this.btnStatus_Click);

            // btnOrder
            this.btnOrder.Location = new System.Drawing.Point(500, 17);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(140, 36);
            this.btnOrder.Text = "📦 สั่งอาหาร";
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);

            // pnlBody
            this.pnlBody.Controls.Add(this.pnlRight);
            this.pnlBody.Controls.Add(this.pnlLeft);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);

            // pnlLeft
            this.pnlLeft.Controls.Add(this.btnAdd);
            this.pnlLeft.Controls.Add(this.listBoxMenu);
            this.pnlLeft.Controls.Add(this.lblMenuTitle);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Width = 300;

            // lblMenuTitle
            this.lblMenuTitle.AutoSize = false;
            this.lblMenuTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMenuTitle.Height = 32;
            this.lblMenuTitle.Name = "lblMenuTitle";
            this.lblMenuTitle.Text = "📋 เมนู";
            this.lblMenuTitle.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);

            // listBoxMenu
            this.listBoxMenu.Location = new System.Drawing.Point(0, 32);
            this.listBoxMenu.Name = "listBoxMenu";
            this.listBoxMenu.Size = new System.Drawing.Size(284, 260);
            this.listBoxMenu.TabIndex = 0;

            // btnAdd
            this.btnAdd.Location = new System.Drawing.Point(0, 300);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(284, 36);
            this.btnAdd.Text = "➕ เพิ่ม";
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // pnlRight
            this.pnlRight.Controls.Add(this.btnRemove);
            this.pnlRight.Controls.Add(this.listBoxCart);
            this.pnlRight.Controls.Add(this.lblCartTitle);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);

            // lblCartTitle
            this.lblCartTitle.AutoSize = false;
            this.lblCartTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCartTitle.Height = 32;
            this.lblCartTitle.Name = "lblCartTitle";
            this.lblCartTitle.Text = "🛒 ตะกร้า";
            this.lblCartTitle.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);

            // listBoxCart
            this.listBoxCart.Location = new System.Drawing.Point(16, 32);
            this.listBoxCart.Name = "listBoxCart";
            this.listBoxCart.Size = new System.Drawing.Size(300, 260);
            this.listBoxCart.TabIndex = 2;

            // btnRemove
            this.btnRemove.Location = new System.Drawing.Point(16, 300);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(300, 36);
            this.btnRemove.Text = "✖ ลบ";
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 480);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize = new System.Drawing.Size(680, 480);
            this.Name = "Customer";
            this.Text = "Customer";

            this.pnlHeader.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.ResumeLayout(false);
        }

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