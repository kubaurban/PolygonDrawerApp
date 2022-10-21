using System;
﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Polygon : IPolygon, IMovable
    {
        public IList<IPoint> Vertices { get; private set; }
        public IList<IEdge> Edges { get; private set; }

        public Polygon(IList<IPoint> vertices)
        {
            Vertices = vertices.ToList();

            Edges = new List<IEdge>();

            var first = Vertices.First();
            var prev = first;
            foreach (var v in Vertices.Skip(1))
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
            var idx = Vertices.IndexOf(p);
            Vertices.RemoveAt(idx);

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
            var newEdges = new List<Edge> { new Edge(e.U, p), new Edge(p, e.V) };
            Edges.ToList().InsertRange(idx, newEdges);

            // insert new point
            Vertices.Insert(idx + 1, p);
        }

        public PointF Center
        {
            get
            {
                var centerVector = Vertices.Aggregate(new Vector2(0, 0), (x, p) => x += new Vector2(p.X, p.Y)) / Vertices.Count;
                return new(centerVector.X, centerVector.Y);
            }
        }
    }
}
