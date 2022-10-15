using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Point : Shape
    {
        public float X { get; set; }
        public float Y { get; set; }
        public PointF Location => new(X, Y);
        public int PolygonId { get; set; }
        public List<int> RelationIds { get; set; }

        public Point(int id) : base(id) 
        {
            RelationIds = new List<int>();
        }

        public override void Move(Vector2 vector)
        {
            X += vector.X;
            Y += vector.Y;
        }
    }
}
