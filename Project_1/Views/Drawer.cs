using System;
using System.Windows.Forms;

namespace Project_1.Views
{
    public partial class Drawer : Form, IDrawer
    {
        public Drawer()
        {
            InitializeComponent();
        }

        private void DeleteModeChecked(object sender, EventArgs e)
        {
            IsBresenham.Enabled = false;
        }

        private void DrawingModeChecked(object sender, EventArgs e)
        {
            IsBresenham.Enabled = true;
        }
    }
}
