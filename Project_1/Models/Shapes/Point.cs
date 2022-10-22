using Project_1.Models.Shapes.Abstract;
using System;
using System.Drawing;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Point : IPoint
    {
        public float X { get; set; }
        public float Y { get; set; }

        public PointF Center => new(X, Y);

        public Point() { }

        public void Move(Vector2 vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        public bool WasClicked(PointF click, int clickRadius) => Math.Abs(X - click.X) <= clickRadius / 2 && Math.Abs(Y - click.Y) <= clickRadius / 2;

        public object Clone()
        {
            return new Point()
            {
                X = X,
                Y = Y
            };
        }
    }
}
