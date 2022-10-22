using System;
using System.Collections.Generic;

namespace Project_1.Models.Shapes
{
    public interface IPolygon : IShape
    {
        IList<IEdge> Edges { get; }
        IEnumerable<IPoint> Vertices { get; }

        void InsertPoint(IEdge e, IPoint p);
        void RemoveVertex(IPoint p);

        List<IEdge> GetNeighborEdges(IPoint p);
    }
}