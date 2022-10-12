
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
            this.components = new System.ComponentModel.Container();
            this.IsBresenham = new System.Windows.Forms.CheckBox();
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.OptionsBox = new System.Windows.Forms.GroupBox();
            this.MoveMode = new System.Windows.Forms.RadioButton();
            this.DeleteMode = new System.Windows.Forms.RadioButton();
            this.DrawingMode = new System.Windows.Forms.RadioButton();
            this.ManageEdgeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.OptionsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // IsBresenham
            // 
            this.IsBresenham.AutoSize = true;
            this.IsBresenham.Location = new System.Drawing.Point(11, 47);
            this.IsBresenham.Name = "IsBresenham";
            this.IsBresenham.Size = new System.Drawing.Size(142, 19);
            this.IsBresenham.TabIndex = 1;
            this.IsBresenham.Text = "Bresenham Algorithm";
            this.IsBresenham.UseVisualStyleBackColor = true;
            // 
            // PictureBox
            // 
            this.PictureBox.Location = new System.Drawing.Point(12, 86);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(1360, 563);
            this.PictureBox.TabIndex = 2;
            this.PictureBox.TabStop = false;
            this.PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.PictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // OptionsBox
            // 
            this.OptionsBox.Controls.Add(this.MoveMode);
            this.OptionsBox.Controls.Add(this.DeleteMode);
            this.OptionsBox.Controls.Add(this.DrawingMode);
            this.OptionsBox.Controls.Add(this.IsBresenham);
            this.OptionsBox.Location = new System.Drawing.Point(12, 4);
            this.OptionsBox.Name = "OptionsBox";
            this.OptionsBox.Size = new System.Drawing.Size(436, 76);
            this.OptionsBox.TabIndex = 3;
            this.OptionsBox.TabStop = false;
            this.OptionsBox.Text = "Options";
            // 
            // MoveMode
            // 
            this.MoveMode.AutoSize = true;
            this.MoveMode.Location = new System.Drawing.Point(330, 22);
            this.MoveMode.Name = "MoveMode";
            this.MoveMode.Size = new System.Drawing.Size(89, 19);
            this.MoveMode.TabIndex = 4;
            this.MoveMode.TabStop = true;
            this.MoveMode.Text = "Move Mode";
            this.MoveMode.UseVisualStyleBackColor = true;
            this.MoveMode.CheckedChanged += new System.EventHandler(this.MoveModeChecked);
            // 
            // DeleteMode
            // 
            this.DeleteMode.AutoSize = true;
            this.DeleteMode.Location = new System.Drawing.Point(174, 22);
            this.DeleteMode.Name = "DeleteMode";
            this.DeleteMode.Size = new System.Drawing.Size(92, 19);
            this.DeleteMode.TabIndex = 3;
            this.DeleteMode.TabStop = true;
            this.DeleteMode.Text = "Delete Mode";
            this.DeleteMode.UseVisualStyleBackColor = true;
            this.DeleteMode.CheckedChanged += new System.EventHandler(this.DeleteModeChecked);
            // 
            // DrawingMode
            // 
            this.DrawingMode.AutoSize = true;
            this.DrawingMode.Checked = true;
            this.DrawingMode.Location = new System.Drawing.Point(11, 22);
            this.DrawingMode.Name = "DrawingMode";
            this.DrawingMode.Size = new System.Drawing.Size(103, 19);
            this.DrawingMode.TabIndex = 2;
            this.DrawingMode.TabStop = true;
            this.DrawingMode.Text = "Drawing Mode";
            this.DrawingMode.UseVisualStyleBackColor = true;
            this.DrawingMode.CheckedChanged += new System.EventHandler(this.DrawingModeChecked);
            // 
            // ManageEdgeMenu
            // 
            this.ManageEdgeMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ManageEdgeMenu.Name = "ManageEdgeMenu";
            this.ManageEdgeMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // Drawer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 661);
            this.Controls.Add(this.OptionsBox);
            this.Controls.Add(this.PictureBox);
            this.Name = "Drawer";
            this.Text = "PolygonDrawer";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.OptionsBox.ResumeLayout(false);
            this.OptionsBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox IsBresenham;
        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.GroupBox OptionsBox;
        private System.Windows.Forms.RadioButton DeleteMode;
        private System.Windows.Forms.RadioButton DrawingMode;
        private System.Windows.Forms.RadioButton MoveMode;
        private System.Windows.Forms.ContextMenuStrip ManageEdgeMenu;
    }
}