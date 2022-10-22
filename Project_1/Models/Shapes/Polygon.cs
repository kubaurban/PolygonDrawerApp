using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Polygon : IPolygon, IMovable
    {
        public IList<IEdge> Edges { get; private set; }
        public IEnumerable<IPoint> Vertices => Edges.Select(x => x.U);

        public Polygon(IList<IPoint> vertices)
        {
            Edges = new List<IEdge>();

            var first = vertices.First();
            var prev = first;
            foreach (var v in vertices.Skip(1))
            {
                Edges.Add(new Edge(prev, v));
                prev = v;
            }
            Edges.Add(new Edge(prev, first));
        }

        public void Move(Vector2 vector)
        {
            foreach (var v in Vertices)
            {
                v.Move(vector);
            }
        }

        public void RemoveVertex(IPoint p)
        {
            var idx = Vertices.ToList().IndexOf(p);

            var prevIdx = (idx - 1 + Edges.Count) % Edges.Count;
            var prevEdge = Edges.ElementAt(prevIdx);
            var nextEdge = Edges.ElementAt(idx);

            // insert newly created edge
            Edges.Insert(prevIdx, new Edge(prevEdge.U, nextEdge.V));

            // remove neighboring edges
            Edges.Remove(prevEdge);
            Edges.Remove(nextEdge);
        }

        public void InsertPoint(IEdge e, IPoint p)
        {
            var idx = Edges.IndexOf(e);

            // remove old edge
            Edges.RemoveAt(idx);

            // insert new edges
            Edges.Insert(idx, new Edge(p, e.V));
            Edges.Insert(idx, new Edge(e.U, p));
        }

        public bool WasClicked(PointF click, int clickRadius)
        {
            var center = Center;
            return Math.Abs(center.X - click.X) <= clickRadius / 2 && Math.Abs(center.Y - click.Y) <= clickRadius / 2;
        }

        public List<IEdge> GetNeighborEdges(IPoint p) => Edges.Where(x => x.U == p || x.V == p).ToList();

        public PointF Center
        {
            get
            {
                var centerVector = Vertices.Aggregate(new Vector2(0, 0), (x, p) => x += new Vector2(p.X, p.Y)) / Edges.Count;
                return new(centerVector.X, centerVector.Y);
            }
        }
    }
}
