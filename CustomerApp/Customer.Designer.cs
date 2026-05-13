namespace CustomerApp
{
    partial class Customer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBoxMenu = new ListBox();
            listBoxCart = new ListBox();
            btnAdd = new Button();
            lblTotal = new Label();
            btnOrder = new Button();
            SuspendLayout();
            // 
            // listBoxMenu
            // 
            listBoxMenu.FormattingEnabled = true;
            listBoxMenu.Location = new Point(113, 49);
            listBoxMenu.Name = "listBoxMenu";
            listBoxMenu.Size = new Size(120, 94);
            listBoxMenu.TabIndex = 0;
            // 
            // listBoxCart
            // 
            listBoxCart.FormattingEnabled = true;
            listBoxCart.Location = new Point(113, 149);
            listBoxCart.Name = "listBoxCart";
            listBoxCart.Size = new Size(120, 94);
            listBoxCart.TabIndex = 1;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(133, 283);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 2;
            btnAdd.Text = "Add >>";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(411, 287);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(45, 15);
            lblTotal.TabIndex = 3;
            lblTotal.Text = "Total: 0";
            // 
            // btnOrder
            // 
            btnOrder.Location = new Point(279, 283);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(75, 23);
            btnOrder.TabIndex = 4;
            btnOrder.Text = "ORDER";
            btnOrder.UseVisualStyleBackColor = true;
            btnOrder.Click += btnOrder_Click;
            // 
            // Customer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnOrder);
            Controls.Add(lblTotal);
            Controls.Add(btnAdd);
            Controls.Add(listBoxCart);
            Controls.Add(listBoxMenu);
            Name = "Customer";
            Text = "Customer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxMenu;
        private ListBox listBoxCart;
        private Button btnAdd;
        private Label lblTotal;
        private Button btnOrder;
    }
}
