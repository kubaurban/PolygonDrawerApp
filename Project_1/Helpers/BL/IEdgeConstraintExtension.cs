using Project_1.Models.Constraints;
using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;
using System.Collections.Generic;

namespace Project_1.Helpers.BL
{
    public static class IEdgeConstraintExtension
    {
        public static IEnumerable<IEdgeConstraint<IEdge>> AsEdgeConstraint(this IEnumerable<Perpendicular> constraints)
        {
            var it = constraints.GetEnumerator();
            while (it.MoveNext())
            {
                yield return it.Current;
            }
        }
    }
}
