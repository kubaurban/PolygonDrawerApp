using System;
using System.Drawing;

namespace Project_1.Models.Shapes
{
    public interface IShape : IMovable
    {
        PointF Center { get; }

    }
}
