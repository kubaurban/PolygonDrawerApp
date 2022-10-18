using Project_1.Models.Constraints;
using System.Collections.Generic;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    /// <summary>
    /// Wrapper for two points, which represents an edge
    /// </summary>
    public class Edge : Shape
    {
        public Point U { get; set; }
        public Point V { get; set; }

        public FixedLength FixedLength { get; set; }
        public List<Perpendicular> Perpendiculars { get; }

        public Edge(Point u, Point v)
        {
            U = u;
            V = v;
            Perpendiculars = new List<Perpendicular>();
        }

        public Polygon Polygon => U.Polygon;

        public int Length => (int)new Vector2(U.X - V.X, U.Y - V.Y).Length();

        public override void Move(Vector2 vector)
        {
            U.Move(vector);
            V.Move(vector);
        }
    }
}
