
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
            this.MakePerpendicularMode = new System.Windows.Forms.RadioButton();
            this.ModifyMode = new System.Windows.Forms.RadioButton();
            this.DeleteMode = new System.Windows.Forms.RadioButton();
            this.DrawingMode = new System.Windows.Forms.RadioButton();
            this.ManageEdgeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RelationsBox = new System.Windows.Forms.GroupBox();
            this.RelationsList = new System.Windows.Forms.ListBox();
            this.ManageEdgeRelationMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.OptionsBox.SuspendLayout();
            this.RelationsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // IsBresenham
            // 
            this.IsBresenham.AutoSize = true;
            this.IsBresenham.Location = new System.Drawing.Point(13, 63);
            this.IsBresenham.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.IsBresenham.Name = "IsBresenham";
            this.IsBresenham.Size = new System.Drawing.Size(175, 24);
            this.IsBresenham.TabIndex = 1;
            this.IsBresenham.Text = "Bresenham Algorithm";
            this.IsBresenham.UseVisualStyleBackColor = true;
            this.IsBresenham.CheckedChanged += new System.EventHandler(this.IsBresenhamCheckedChanged);
            // 
            // PictureBox
            // 
            this.PictureBox.Location = new System.Drawing.Point(14, 115);
            this.PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(1554, 751);
            this.PictureBox.TabIndex = 2;
            this.PictureBox.TabStop = false;
            this.PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.PictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // OptionsBox
            // 
            this.OptionsBox.Controls.Add(this.MakePerpendicularMode);
            this.OptionsBox.Controls.Add(this.ModifyMode);
            this.OptionsBox.Controls.Add(this.DeleteMode);
            this.OptionsBox.Controls.Add(this.DrawingMode);
            this.OptionsBox.Controls.Add(this.IsBresenham);
            this.OptionsBox.Location = new System.Drawing.Point(14, 5);
            this.OptionsBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.OptionsBox.Name = "OptionsBox";
            this.OptionsBox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.OptionsBox.Size = new System.Drawing.Size(480, 101);
            this.OptionsBox.TabIndex = 3;
            this.OptionsBox.TabStop = false;
            this.OptionsBox.Text = "Options";
            // 
            // MakePerpendicularMode
            // 
            this.MakePerpendicularMode.AutoSize = true;
            this.MakePerpendicularMode.Location = new System.Drawing.Point(306, 29);
            this.MakePerpendicularMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MakePerpendicularMode.Name = "MakePerpendicularMode";
            this.MakePerpendicularMode.Size = new System.Drawing.Size(162, 24);
            this.MakePerpendicularMode.TabIndex = 5;
            this.MakePerpendicularMode.TabStop = true;
            this.MakePerpendicularMode.Text = "Make perpendicular";
            this.MakePerpendicularMode.UseVisualStyleBackColor = true;
            this.MakePerpendicularMode.CheckedChanged += new System.EventHandler(this.MakePerpendicularModeChecked);
            // 
            // ModifyMode
            // 
            this.ModifyMode.AutoSize = true;
            this.ModifyMode.Location = new System.Drawing.Point(211, 29);
            this.ModifyMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ModifyMode.Name = "ModifyMode";
            this.ModifyMode.Size = new System.Drawing.Size(77, 24);
            this.ModifyMode.TabIndex = 4;
            this.ModifyMode.TabStop = true;
            this.ModifyMode.Text = "Modify";
            this.ModifyMode.UseVisualStyleBackColor = true;
            this.ModifyMode.CheckedChanged += new System.EventHandler(this.ModifyModeChecked);
            // 
            // DeleteMode
            // 
            this.DeleteMode.AutoSize = true;
            this.DeleteMode.Location = new System.Drawing.Point(109, 29);
            this.DeleteMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DeleteMode.Name = "DeleteMode";
            this.DeleteMode.Size = new System.Drawing.Size(74, 24);
            this.DeleteMode.TabIndex = 3;
            this.DeleteMode.TabStop = true;
            this.DeleteMode.Text = "Delete";
            this.DeleteMode.UseVisualStyleBackColor = true;
            this.DeleteMode.CheckedChanged += new System.EventHandler(this.DeleteModeChecked);
            // 
            // DrawingMode
            // 
            this.DrawingMode.AutoSize = true;
            this.DrawingMode.Checked = true;
            this.DrawingMode.Location = new System.Drawing.Point(13, 29);
            this.DrawingMode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DrawingMode.Name = "DrawingMode";
            this.DrawingMode.Size = new System.Drawing.Size(65, 24);
            this.DrawingMode.TabIndex = 2;
            this.DrawingMode.TabStop = true;
            this.DrawingMode.Text = "Draw";
            this.DrawingMode.UseVisualStyleBackColor = true;
            this.DrawingMode.CheckedChanged += new System.EventHandler(this.DrawingModeChecked);
            // 
            // ManageEdgeMenu
            // 
            this.ManageEdgeMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ManageEdgeMenu.Name = "ManageEdgeMenu";
            this.ManageEdgeMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // RelationsBox
            // 
            this.RelationsBox.Controls.Add(this.RelationsList);
            this.RelationsBox.Location = new System.Drawing.Point(512, 5);
            this.RelationsBox.Name = "RelationsBox";
            this.RelationsBox.Size = new System.Drawing.Size(1056, 100);
            this.RelationsBox.TabIndex = 4;
            this.RelationsBox.TabStop = false;
            this.RelationsBox.Text = "Relations";
            this.RelationsBox.Visible = false;
            // 
            // RelationsList
            // 
            this.RelationsList.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.RelationsList.ForeColor = System.Drawing.SystemColors.Desktop;
            this.RelationsList.FormattingEnabled = true;
            this.RelationsList.ItemHeight = 20;
            this.RelationsList.Location = new System.Drawing.Point(19, 24);
            this.RelationsList.Name = "RelationsList";
            this.RelationsList.Size = new System.Drawing.Size(1017, 64);
            this.RelationsList.TabIndex = 0;
            this.RelationsList.SelectedValueChanged += new System.EventHandler(this.OnSelectedRelationChanged);
            this.RelationsList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnRelationsListMouseDown);
            // 
            // ManageEdgeRelationMenu
            // 
            this.ManageEdgeRelationMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ManageEdgeRelationMenu.Name = "ManageEdgeRelationMenu";
            this.ManageEdgeRelationMenu.Size = new System.Drawing.Size(211, 32);
            // 
            // Drawer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1582, 881);
            this.Controls.Add(this.RelationsBox);
            this.Controls.Add(this.OptionsBox);
            this.Controls.Add(this.PictureBox);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Drawer";
            this.Text = "PolygonDrawer";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.OptionsBox.ResumeLayout(false);
            this.OptionsBox.PerformLayout();
            this.RelationsBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox IsBresenham;
        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.GroupBox OptionsBox;
        private System.Windows.Forms.RadioButton DeleteMode;
        private System.Windows.Forms.RadioButton DrawingMode;
        private System.Windows.Forms.RadioButton ModifyMode;
        private System.Windows.Forms.ContextMenuStrip ManageEdgeMenu;
        private System.Windows.Forms.RadioButton MakePerpendicularMode;
        private System.Windows.Forms.GroupBox RelationsBox;
        private System.Windows.Forms.ListBox RelationsList;
        private System.Windows.Forms.ContextMenuStrip ManageEdgeRelationMenu;
    }
}