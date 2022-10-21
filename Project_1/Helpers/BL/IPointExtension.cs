using Project_1.Models.Shapes;

namespace Project_1.Helpers.BL
{
    public static class IPointExtension
    {
        public static IPoint GetNeighbor(this IPoint u, IEdge e)
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
