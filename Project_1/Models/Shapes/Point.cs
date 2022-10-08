using System;

namespace Project_1.Models.Shapes
{
    public class Point : Shape
    {
        private static readonly int _pointWidth = 8;

        public float X { get; set; }
        public float Y { get; set; }
        public int PolygonId { get; set; }

        public static int PointWidth => _pointWidth;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool IsInside(System.Drawing.Point point) => Math.Abs(point.X - X) <= PointWidth / 2 && Math.Abs(point.Y - Y) <= PointWidth / 2;

        public static implicit operator System.Drawing.PointF(Point p) => new(p.X, p.Y);
        public static implicit operator Point(System.Drawing.Point p) => new(p.X, p.Y);
    }
}
