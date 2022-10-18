using Project_1.Models.Constraints;
using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IConstraintRepository
    {
        FixedLength AddFixedLength(Edge edge, int length);
        Perpendicular AddPerpendicular(Edge constrained, Edge constraint);
        List<FixedLength> GetFixedLengths();
        List<Perpendicular> GetPerpendiculars();
        FixedLength RemoveFixedLength(FixedLength relation);
        Perpendicular RemovePerpendicular(Perpendicular relation);
    }
}
