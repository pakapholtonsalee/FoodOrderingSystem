namespace RestaurantApp
{
    partial class Restaurant
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
            lblTitle = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            lblOrderCount = new System.Windows.Forms.Label();
            listBoxOrders = new System.Windows.Forms.ListBox();
            pnlButtons = new System.Windows.Forms.Panel();
            btnPreparing = new System.Windows.Forms.Button();
            btnCompleted = new System.Windows.Forms.Button();
            pnlHeader = new System.Windows.Forms.Panel();

            pnlHeader.SuspendLayout();
            pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // pnlHeader
            pnlHeader.BackColor = System.Drawing.Color.FromArgb(56, 142, 60);
            pnlHeader.Controls.Add(lblStatus);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Height = 70;

            // lblTitle
            lblTitle.AutoSize = false;
            lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.Text = "🍽️  Restaurant Dashboard";
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblStatus
            lblStatus.AutoSize = true;
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(200, 255, 200);
            lblStatus.Location = new System.Drawing.Point(10, 6);
            lblStatus.Text = "🔴 กำลังเชื่อมต่อ...";

            // lblOrderCount
            lblOrderCount.AutoSize = false;
            lblOrderCount.Dock = System.Windows.Forms.DockStyle.Top;
            lblOrderCount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblOrderCount.ForeColor = System.Drawing.Color.FromArgb(60, 100, 60);
            lblOrderCount.Height = 32;
            lblOrderCount.Padding = new System.Windows.Forms.Padding(12, 6, 0, 0);
            lblOrderCount.Text = "ออเดอร์ทั้งหมด: 0 รายการ";
            lblOrderCount.BackColor = System.Drawing.Color.FromArgb(232, 245, 233);

            // listBoxOrders
            listBoxOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            listBoxOrders.Font = new System.Drawing.Font("Segoe UI", 10F);
            listBoxOrders.ItemHeight = 36;
            listBoxOrders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            listBoxOrders.BackColor = System.Drawing.Color.White;

            // pnlButtons
            pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlButtons.Height = 60;
            pnlButtons.BackColor = System.Drawing.Color.FromArgb(232, 245, 233);
            pnlButtons.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            pnlButtons.Controls.Add(btnCompleted);
            pnlButtons.Controls.Add(btnPreparing);

            // btnPreparing
            btnPreparing.BackColor = System.Drawing.Color.FromArgb(33, 150, 243);
            btnPreparing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnPreparing.FlatAppearance.BorderSize = 0;
            btnPreparing.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnPreparing.ForeColor = System.Drawing.Color.White;
            btnPreparing.Location = new System.Drawing.Point(12, 10);
            btnPreparing.Size = new System.Drawing.Size(180, 38);
            btnPreparing.Text = "👨‍🍳  กำลังจัดเตรียม";
            btnPreparing.Cursor = System.Windows.Forms.Cursors.Hand;
            btnPreparing.Click += new System.EventHandler(this.btnPreparing_Click);

            // btnCompleted
            btnCompleted.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            btnCompleted.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCompleted.FlatAppearance.BorderSize = 0;
            btnCompleted.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnCompleted.ForeColor = System.Drawing.Color.White;
            btnCompleted.Location = new System.Drawing.Point(204, 10);
            btnCompleted.Size = new System.Drawing.Size(160, 38);
            btnCompleted.Text = "✅  เสร็จแล้ว";
            btnCompleted.Cursor = System.Windows.Forms.Cursors.Hand;
            btnCompleted.Click += new System.EventHandler(this.btnCompleted_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 520);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(listBoxOrders);
            this.Controls.Add(lblOrderCount);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);
            this.Name = "Restaurant";
            this.Text = "Restaurant Dashboard";

            pnlHeader.ResumeLayout(false);
            pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblOrderCount;
        private System.Windows.Forms.ListBox listBoxOrders;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnPreparing;
        private System.Windows.Forms.Button btnCompleted;
    }
}

