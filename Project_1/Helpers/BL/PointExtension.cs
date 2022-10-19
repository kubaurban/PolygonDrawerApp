using Project_1.Models.Constraints;
using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Project_1.Helpers.BL
{
    public static class PointExtension
    {
        public static List<Edge> GetEdges(this Point u) => u.Polygon.Edges.Where(x => x.U == u || x.V == u).ToList();

        public static Point GetNeighbor(this Point u, Edge e)
        {
            if (e.U == u)
            {
                return e.V;
            }
            else if (e.V == u)
            {
                return e.U;
            }
            else
            {
                return null;
            }
        }
    }
}
