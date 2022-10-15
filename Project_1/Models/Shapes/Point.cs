using System.Collections.Generic;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Point : Shape
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int PolygonId { get; set; }
        public List<int> RelationIds { get; set; }

        public Point(int id) : base(id) 
        {
            RelationIds = new List<int>();
        }

        private Point(float x, float y) : base()
        {
            X = x;
            Y = y;
            RelationIds = new List<int>();
        }

        public override void Move(Vector2 vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        public static implicit operator System.Drawing.PointF(Point p) => new(p.X, p.Y);
        public static implicit operator Point(System.Drawing.PointF p) => new(p.X, p.Y);
    }
}
