using Project_1.Models.Shapes;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Point = Project_1.Models.Shapes.Point;

namespace Project_1.Views
{
    public partial class Drawer : Form, IDrawer
    {
        private readonly Bitmap _drawArea;
        private readonly Pen _blackPen;
        private readonly Brush _blackBrush;
        private readonly float _pointWidth;

        public Bitmap DrawArea => _drawArea;
        public Graphics Graphics => Graphics.FromImage(DrawArea);
        public Pen BlackPen => _blackPen;
        public Brush BlackBrush => _blackBrush;
        public float PointWidth => _pointWidth;

        public Drawer()
        {
            InitializeComponent();

            _drawArea = new Bitmap(PictureBox.Width, PictureBox.Height);
            _blackPen = new Pen(Color.Black);
            _blackBrush = new SolidBrush(Color.Black);
            _pointWidth = 0.25f;

            PictureBox.Image = DrawArea;
            ClearArea();
            RefreshArea();
        }

        private void DeleteModeChecked(object sender, EventArgs e)
        {
            IsBresenham.Enabled = false;
        }

        private void DrawingModeChecked(object sender, EventArgs e)
        {
            IsBresenham.Enabled = true;
        }

        public void DrawLine(Point p1, Point p2)
        {
            using var g = Graphics;
            g.DrawLine(BlackPen, p1, p2);
        }

        public void DrawPoint(Point p)
        {
            using var g = Graphics;
            g.FillRectangle(BlackBrush, p.X - PointWidth / 2, p.Y - PointWidth / 2, PointWidth, PointWidth);
        }

        public void DrawPolygon(Polygon polygon)
        {
            using var g = Graphics;

            var first = polygon.Vertices.First();
            DrawPoint(first);
            var prev = first;
            foreach (var v in polygon.Vertices.Skip(1))
            {
                DrawPoint(v);
                DrawLine(prev, v);
                prev = v;
            }
            DrawLine(prev, first);
        }

        public void ClearArea()
        {
            using var g = Graphics;
            g.Clear(Color.White);
        }

        public void RefreshArea()
        {
            PictureBox.Refresh();
        }
    }
}
