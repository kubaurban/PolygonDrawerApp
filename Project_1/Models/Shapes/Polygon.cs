using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Polygon : Shape
    {
        private List<Point> _vertices;

        public List<Point> Vertices {
            get => _vertices;
            set
            {
                _vertices = value;
                _vertices.ForEach(x => x.Polygon = this);
            }
        }

        public List<Edge> Edges
        {
            get
            {
                var edges = new List<Edge>();

                var first = Vertices.First();
                var prev = first;
                foreach (var v in Vertices.Skip(1))
                {
                    edges.Add(new(prev, v));
                    prev = v;
                }
                edges.Add(new(prev, first));

                return edges;
            }
        }

        public override void Move(Vector2 vector)
        {
            Vertices.ForEach(x => x.Move(vector));
        }

        public void RemoveVertex(Point p)
        {
            Vertices.Remove(p);
        }

        public void InsertPoint(Edge e, Point p)
        {
            var vIdx = Vertices.IndexOf(e.V);
            p.Polygon = this;
            Vertices.Insert(vIdx, p);
        }

        public PointF GravityCenterPoint {
            get
            {
                var centerVector = Vertices.Aggregate(new Vector2(0, 0), (x, p) => x += new Vector2(p.X, p.Y)) / Vertices.Count;
                return new(centerVector.X, centerVector.Y);
            }
        }
    }
}
