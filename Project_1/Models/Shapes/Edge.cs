using System;
using System.Numerics;

namespace Project_1.Models.Shapes
{
    public class Edge : Shape
    {
        public Point U { get; set; }
        public Point V { get; set; }

        public Edge(Point u, Point v) : base()
        {
            U = u;
            V = v;
        }

        public int PolygonId => U.PolygonId;

        public int Length => (int)new Vector2(U.X - V.X, U.Y - V.Y).Length();

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
