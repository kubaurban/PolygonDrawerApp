﻿using Project_1.Models.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Point = Project_1.Models.Shapes.Point;

namespace Project_1.Views
{
    public partial class Drawer : Form, IDrawer
    {
        private static readonly int _pointWidth = 8;
        private static readonly int _moveIconWidth = 20;

        private readonly Bitmap _drawArea;
        private readonly Pen _blackPen;
        private readonly Brush _blackBrush;
        private readonly bool _isLeftMouseClicked;

        public event MouseEventHandler LeftMouseDownHandler;
        public event MouseEventHandler RightMouseDownHandler;
        public event MouseEventHandler MouseDownMoveHandler;

        public Bitmap DrawArea => _drawArea;
        public Graphics Graphics => Graphics.FromImage(DrawArea);
        public Pen BlackPen => _blackPen;
        public Brush BlackBrush => _blackBrush;
        public bool IsLeftMouseDown => _isLeftMouseClicked;
        public bool IsInDrawingMode => DrawingMode?.Checked ?? true;
        public bool IsInDeleteMode => DeleteMode?.Checked ?? false;
        public bool IsInMoveMode => MoveMode?.Checked ?? false;
        public static int PointWidth => _pointWidth;
        public static int MoveIconWidth => _moveIconWidth;
        public string MoveIconFilePath => "../../../../Resources/move.ico";

        public Drawer()
        {
            InitializeComponent();

            _drawArea = new Bitmap(PictureBox.Width, PictureBox.Height);
            _blackPen = new Pen(Color.Black);
            _blackBrush = new SolidBrush(Color.Black);
            _isLeftMouseClicked = false;

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

            // draw gravity center point
            DrawGrabIcon(polygon.GravityCenterPoint);
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
            g.DrawIcon(new(MoveIconFilePath), new(point.X, point.Y, MoveIconWidth, MoveIconWidth));
        }

        public static bool IsInside(PointF click, Point point, int pointWidth) 
            => Math.Abs(point.X - click.X) <= pointWidth / 2 && Math.Abs(point.Y - click.Y) <= pointWidth / 2;

        public void ClearArea()
        {
            using var g = Graphics;
            g.Clear(Color.White);
        }

        public void RefreshArea()
        {
            PictureBox.Refresh();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
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
    }
}
