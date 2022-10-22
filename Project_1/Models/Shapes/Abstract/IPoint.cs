using System;
using System.Numerics;

namespace Project_1.Models.Shapes.Abstract
{
    public interface IPoint : IShape, ICloneable
    {
        float X { get; set; }
        float Y { get; set; }

        public static Vector2 operator -(IPoint p1, IPoint p2)
        {
            return new()
            {
                X = p1.X - p2.X,
                Y = p1.Y - p2.Y
            };
        }
    }
}