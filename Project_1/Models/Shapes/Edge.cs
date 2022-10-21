using System.Drawing;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Edge : IEdge
    {
        public IPoint U { get; set; }
        public IPoint V { get; set; }

        public PointF Center => new((U.X + V.X) / 2, (U.Y + V.Y) / 2);

        public Edge(IPoint u, IPoint v)
        {
            U = u;
            V = v;
        }

        public int Length => (int)new Vector2(U.X - V.X, U.Y - V.Y).Length();

        public void Move(Vector2 vector)
        {
            U.Move(vector);
            V.Move(vector);
        }
    }
}
