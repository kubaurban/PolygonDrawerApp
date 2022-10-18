using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms.VisualStyles;

namespace Project_1.Models.Shapes
{
    public class Polygon : Shape
    {
        public List<Point> Vertices { get; set; }
        public List<Edge> Edges { get; private set; }

        public Polygon(List<Point> vertices)
        {
            Vertices = vertices.ToList();
            Vertices.ForEach(x => x.Polygon = this);

            Edges = new List<Edge>();

            var first = Vertices.First();
            var prev = first;
            foreach (var v in Vertices.Skip(1))
            {
                Edges.Add(new(prev, v));
                prev = v;
            }
            Edges.Add(new(prev, first));
        }

        public override void Move(Vector2 vector)
        {
            Vertices.ForEach(x => x.Move(vector));
        }

        public void RemoveVertex(Point p)
        {
            var idx = Vertices.IndexOf(p);
            Vertices.RemoveAt(idx);

            // remove neighboring edges
            var prevIdx = (idx - 1 + Edges.Count) % Edges.Count;
            var u = Edges.ElementAt(prevIdx).U;
            var v = Edges.ElementAt(idx).V;

            // insert newly created edge
            Edges.Insert(prevIdx, new(u, v));
        }

        public void InsertPoint(Edge e, Point p)
        {
            var idx = Edges.IndexOf(e);

            // remove old edge
            Edges.RemoveAt(idx);

            // insert new edges
            var newEdges = new List<Edge> { new(e.U, p), new(p, e.V) };
            Edges.InsertRange(idx, newEdges);

            // insert new point
            p.Polygon = this;
            Vertices.Insert(idx + 1, p);
        }

        public PointF GravityCenterPoint
        {
            get
            {
                var centerVector = Vertices.Aggregate(new Vector2(0, 0), (x, p) => x += new Vector2(p.X, p.Y)) / Vertices.Count;
                return new(centerVector.X, centerVector.Y);
            }
        }
    }
}
