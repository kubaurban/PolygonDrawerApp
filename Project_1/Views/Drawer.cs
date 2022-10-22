using Project_1.Helpers.UI;
using Project_1.Models.Shapes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Project_1.Views
{
    delegate void LineDrawer(PointF p1, PointF p2);

    public partial class Drawer : Form, IDrawer
    {
        #region Private fields

        private static readonly int _pointWidth = 8;
        private static readonly int _edgeWidth = 4;
        private static readonly int _moveIconWidth = 20;

        private readonly Bitmap _drawArea;
        private readonly Pen _blackPen;
        private readonly Brush _blackBrush;

        #endregion

        #region User action handlers

        public event MouseEventHandler LeftMouseDownHandler;
        public event MouseEventHandler LeftMouseUpHandler;
        public event MouseEventHandler RightMouseDownHandler;
        public event MouseEventHandler MouseDownMoveHandler;
        public event MouseEventHandler MouseUpMoveHandler;

        #endregion

        #region BL handlers

        public event EventHandler ModeChangedHandler;
        public event EventHandler EdgeInsertPointClickedHandler;
        public event EventHandler EdgeSetLengthClickedHandler;

        #endregion

        public Bitmap DrawArea => _drawArea;
        public Graphics Graphics => Graphics.FromImage(DrawArea);
        public Pen BlackPen => _blackPen;
        public Brush BlackBrush => _blackBrush;
        public DrawerMode Mode { get; set; }
        public bool IsLeftMouseDown { get; set; }
        private LineDrawer LineDrawer { get; set; }
        public static int PointWidth => _pointWidth;
        public static int EdgeWidth => _edgeWidth;
        public static int MoveIconWidth => _moveIconWidth;
        public string MoveIconFilePath => "../../../../Resources/move.ico";

        public Drawer()
        {
            InitializeComponent();
            InitManageEdgeMenuItems();

            _drawArea = new Bitmap(PictureBox.Width, PictureBox.Height);
            _blackPen = new Pen(Color.Black);
            _blackBrush = new SolidBrush(Color.Black);

            Mode = DrawerMode.Draw;
            IsLeftMouseDown = false;
            LineDrawer = DrawLineLibrary;
            DrawingMode.CheckedChanged += OnDrawingModeChecked;
            DeleteMode.CheckedChanged += OnDeleteModeChecked;
            MoveMode.CheckedChanged += OnMoveModeChecked;
            MakePerpendicularMode.CheckedChanged += OnMakePerpendicularModeChecked;

            PictureBox.Image = DrawArea;
            ClearArea();
            RefreshArea();
        }

        #region Manage edge menu
        private void InitManageEdgeMenuItems()
        {
            var addMiddle = new ToolStripMenuItem("Insert point", null, new EventHandler(OnEdgeInsertPoint));
            var setLength = new ToolStripMenuItem("Set fixed length", null, new EventHandler(OnEdgeSetFixedLength));
            ManageEdgeMenu.Items.AddRange(new ToolStripItem[] { addMiddle, setLength });
        }

        public void ShowManageEdgeMenu(PointF point)
        {
            ManageEdgeMenu.Show(PictureBox, new System.Drawing.Point((int)point.X, (int)point.Y));
        }
        #endregion

        #region Bresenham logic
        private void DisableBresenhamAlgorithm()
        {
            IsBresenham.Enabled = false;
        }

        private void EnableBresenhamAlgorithm()
        {
            IsBresenham.Enabled = true;
        }

        private void DeleteModeChecked(object sender, EventArgs e)
        {
            DisableBresenhamAlgorithm();
        }

        private void DrawingModeChecked(object sender, EventArgs e)
        {
            EnableBresenhamAlgorithm();
        }

        private void MoveModeChecked(object sender, EventArgs e)
        {
            DisableBresenhamAlgorithm();
        }

        private void MakePerpendicularModeChecked(object sender, EventArgs e)
        {
            DisableBresenhamAlgorithm();
        }

        private void IsBresenhamCheckedChanged(object sender, EventArgs e)
        {
            if (IsBresenham.Checked)
            {
                LineDrawer = DrawLineBresenham;
            }
            else
            {
                LineDrawer = DrawLineLibrary;
            }
        }
        #endregion

        #region Drawing shapes
        public void DrawLine(PointF p1, PointF p2)
        {
            LineDrawer?.Invoke(p1, p2);
        }

        private void DrawLineLibrary(PointF p1, PointF p2)
        {
            using var g = Graphics;
            g.DrawLine(BlackPen, p1, p2);
        }

        private void DrawLineBresenham(PointF p1, PointF p2)
        {
            var dx = Math.Abs((int)p2.X - (int)p1.X);
            var dy = Math.Abs((int)p2.Y - (int)p1.Y);
            var d_horizontal = (p1.X < p2.X) ? 1 : p1.X == p2.X ? 0 : -1;
            var d_vertical = (p1.Y < p2.Y) ? 1 : p1.Y == p2.Y ? 0 : -1;

            var x = (int)p1.X;
            var y = (int)p1.Y;
            DrawArea.SetPixel(x, y, Color.Black);

            if (dx > dy)
            {
                int d = 2 * dy - dx;
                while (x != p2.X)
                {
                    if (d < 0) // choose E
                    {
                        d += 2 * dy;
                        x += d_horizontal;
                    }
                    else // choose NE
                    {
                        d += (dy - dx) * 2;
                        x += d_horizontal;
                        y += d_vertical;
                    }
                    DrawArea.SetPixel(x, y, Color.Black);
                }
            }
            else
            {
                int d = 2 * dx - dy;
                while (y != p2.Y)
                {
                    if (d < 0) // choose E
                    {
                        d += 2 * dx;
                        y += d_vertical;
                    }
                    else // choose NE
                    {
                        d += (dx - dy) * 2;
                        x += d_horizontal;
                        y += d_vertical;
                    }
                    DrawArea.SetPixel(x, y, Color.Black);
                }
            }
        }

        public void DrawPoint(PointF p)
        {
            using var g = Graphics;
            g.FillRectangle(BlackBrush, p.X - PointWidth / 2, p.Y - PointWidth / 2, PointWidth, PointWidth);
        }

        public void DrawPolygon(IPolygon polygon)
        {
            using var g = Graphics;

            var first = polygon.Vertices.First();
            DrawPoint(first.Center);
            var prev = first;
            foreach (var v in polygon.Vertices.Skip(1))
            {
                DrawPoint(v.Center);
                DrawLineLibrary(prev.Center, v.Center);
                prev = v;
            }
            DrawLineLibrary(prev.Center, first.Center);

            if (Mode == DrawerMode.Move)
            {
                // draw gravity center point
                DrawGrabIcon(polygon.Center);
            }
        }

        public void DrawPolygons(IEnumerable<IPolygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                DrawPolygon(polygon);
            }
        }

        public void DrawGrabIcon(PointF point)
        {
            using var g = Graphics;
            g.DrawIcon(new(MoveIconFilePath), new((int)point.X - MoveIconWidth / 2, (int)point.Y - MoveIconWidth / 2, MoveIconWidth, MoveIconWidth));
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
        #endregion

        #region Event handling functions
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    IsLeftMouseDown = true;
                    LeftMouseDownHandler?.Invoke(sender, e);
                    break;
                case MouseButtons.Right:
                    RightMouseDownHandler?.Invoke(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsLeftMouseDown)
            {
                MouseDownMoveHandler?.Invoke(sender, e);
            }
            else
            {
                MouseUpMoveHandler?.Invoke(sender, e);
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsLeftMouseDown = false;
                LeftMouseUpHandler?.Invoke(sender, e);
            }
        }

        private void OnDrawingModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.Draw;
            OnModeChanged(sender, e);
        }

        private void OnDeleteModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.Delete;
            OnModeChanged(sender, e);
        }

        private void OnMoveModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.Move;
            OnModeChanged(sender, e);
        }

        private void OnMakePerpendicularModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.MakePerpendicular;
            OnModeChanged(sender, e);
        }

        private void OnModeChanged(object sender, EventArgs e)
        {
            ModeChangedHandler?.Invoke(sender, e);
        }

        private void OnEdgeInsertPoint(object sender, EventArgs e)
        {
            EdgeInsertPointClickedHandler?.Invoke(sender, e);
        }

        private void OnEdgeSetFixedLength(object sender, EventArgs e)
        {
            EdgeSetLengthClickedHandler?.Invoke(sender, e);
        }
        #endregion
    }
}