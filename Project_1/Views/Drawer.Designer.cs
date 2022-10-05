
namespace Project_1.Views
{
    partial class Drawer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.IsBresenham = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // IsBresenham
            // 
            this.IsBresenham.AutoSize = true;
            this.IsBresenham.Location = new System.Drawing.Point(12, 12);
            this.IsBresenham.Name = "IsBresenham";
            this.IsBresenham.Size = new System.Drawing.Size(142, 19);
            this.IsBresenham.TabIndex = 1;
            this.IsBresenham.Text = "Bresenham Algorithm";
            this.IsBresenham.UseVisualStyleBackColor = true;
            // 
            // Drawer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 661);
            this.Controls.Add(this.IsBresenham);
            this.Name = "Drawer";
            this.Text = "PolygonDrawer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox IsBresenham;
    }
}