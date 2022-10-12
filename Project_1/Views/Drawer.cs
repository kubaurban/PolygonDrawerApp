﻿using Project_1.Helpers.UI;
using Project_1.Models.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Point = Project_1.Models.Shapes.Point;

namespace Project_1.Views
{
    public partial class Drawer : Form, IDrawer
    {
        private static readonly int _pointWidth = 8;
        private static readonly int _edgeWidth = 4;
        private static readonly int _moveIconWidth = 20;

        private readonly Bitmap _drawArea;
        private readonly Pen _blackPen;
        private readonly Brush _blackBrush;

        public event MouseEventHandler LeftMouseDownHandler;
        public event MouseEventHandler LeftMouseUpHandler;
        public event MouseEventHandler RightMouseDownHandler;
        public event MouseEventHandler MouseDownMoveHandler;

        public event EventHandler ModeChangedHandler;
        public event EventHandler EdgeInsertPointClickedHandler;
        public event EventHandler EdgeSaveLengthClickedHandler;

        public Bitmap DrawArea => _drawArea;
        public Graphics Graphics => Graphics.FromImage(DrawArea);
        public Pen BlackPen => _blackPen;
        public Brush BlackBrush => _blackBrush;
        public DrawerMode Mode { get; set; }
        public bool IsLeftMouseDown { get; set; }
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
            DrawingMode.CheckedChanged += OnDrawingModeChecked;
            DeleteMode.CheckedChanged += OnDeleteModeChecked;
            MoveMode.CheckedChanged += OnMoveModeChecked;
            MakePerpendicularMode.CheckedChanged += OnMakePerpendicularModeChecked;

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

        private void MoveModeChecked(object sender, EventArgs e)
        {
            IsBresenham.Enabled = false;
        }

        private void InitManageEdgeMenuItems()
        {
            var addMiddle = new ToolStripMenuItem("Insert point", null, new EventHandler(OnEdgeInsertPoint));
            var setLength = new ToolStripMenuItem("Set fixed length", null, new EventHandler(OnEdgeSetFixedLength));
            ManageEdgeMenu.Items.AddRange(new ToolStripItem[]{ addMiddle, setLength });
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

            if (Mode == DrawerMode.Move)
            {
                // draw gravity center point
                DrawGrabIcon(polygon.GravityCenterPoint.ToPoint());
            }
        }

        public void DrawPolygons(IEnumerable<Polygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                DrawPolygon(polygon);
            }
        }

        public void DrawGrabIcon(System.Drawing.Point point)
        {
            using var g = Graphics;
            g.DrawIcon(new(MoveIconFilePath), new(point.X - MoveIconWidth / 2, point.Y - MoveIconWidth / 2, MoveIconWidth, MoveIconWidth));
        }

        public static bool IsInsidePoint(PointF click, Point point, int pointWidth)
            => Math.Abs(point.X - click.X) <= pointWidth / 2 && Math.Abs(point.Y - click.Y) <= pointWidth / 2;

        public static bool IsInsideEdge(PointF click, Edge edge)
        {
            var u = (PointF)edge.U;
            var v = (PointF)edge.V;

            var uv = new Vector2(v.X - u.X, v.Y - u.Y);
            var a = uv.Length() / PointWidth;
            uv /= a;

            var uvPerpendicular = new Vector2(v.Y - u.Y, u.X - v.X);
            var b = uvPerpendicular.Length() / EdgeWidth;
            uvPerpendicular /= b;

            var polygon = new List<PointF>
            {
                new(u.ToVector2() + uv + uvPerpendicular),
                new(v.ToVector2() - uv + uvPerpendicular),
                new(v.ToVector2() - uv - uvPerpendicular),
                new(u.ToVector2() + uv - uvPerpendicular)
            };

            // code reused from https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y < click.Y && polygon[j].Y >= click.Y || polygon[j].Y < click.Y && polygon[i].Y >= click.Y)
                {
                    if (polygon[i].X + (click.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < click.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
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

        public void ShowManageEdgeMenu(System.Drawing.Point point)
        {
            ManageEdgeMenu.Show(PictureBox, point);
        }

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
            var lengthInputDialog = new LengthInputDialog();
            if (lengthInputDialog.ShowDialog() == DialogResult.OK  && lengthInputDialog.InputLength > 0)
            {
                EdgeSaveLengthClickedHandler?.Invoke(sender, new SaveLengthArgs(lengthInputDialog.InputLength));
            }
            lengthInputDialog.Close();
            lengthInputDialog.Dispose();
        }
    }
}
