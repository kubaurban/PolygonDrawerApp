using Project_1.Models.Constraints;
using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IConstraintRepository
    {
        FixedLength AddFixedLength(IEdge edge, int length);
        Perpendicular AddPerpendicular(IEdge constrained, IEdge constraint);
        List<FixedLength> GetFixedLengths();
        List<Perpendicular> GetPerpendiculars();
        FixedLength RemoveFixedLength(FixedLength relation);
        void RemovePerpendiculars(IList<Perpendicular> relations);
    }
}
