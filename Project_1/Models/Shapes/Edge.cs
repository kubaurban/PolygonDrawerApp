using System;
using System.Collections.Generic;
using System.Linq;
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

        public Edge(Point u, Point v)
        {
            U = u;
            V = v;
        }

        public Polygon Polygon => U.Polygon;

        public int Length => (int)new Vector2(U.X - V.X, U.Y - V.Y).Length();

        public List<int> RelationIds => U.RelationIds.Intersect(V.RelationIds).ToList();

        public override void Move(Vector2 vector)
        {
            U.Move(vector);
            V.Move(vector);
        }

        public void AddRelation(int id)
        {
            U.RelationIds.Add(id);
            V.RelationIds.Add(id);
        }
    }
}
