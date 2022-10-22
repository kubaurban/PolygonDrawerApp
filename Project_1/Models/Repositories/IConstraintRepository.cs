using Project_1.Models.Constraints;
using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IConstraintRepository
    {
        // managing fixed length constraints
        FixedLength AddFixedLength(IEdge edge, int length);
        FixedLength RemoveFixedLength(FixedLength relation);
        void RemoveFixedLengthFor(IEdge edge);
        List<FixedLength> GetAllFixedLengths();
        FixedLength GetFixedLengthFor(IEdge edge);

        // managing perpendicular constraints
        Perpendicular AddPerpendicular(IEdge constrained, IEdge constraint);
        void RemovePerpendiculars(IList<Perpendicular> relations);
        void RemovePerpendicularsFor(IEdge edge);
        List<Perpendicular> GetAllPerpendiculars();
        List<Perpendicular> GetPerpendicularsFor(IEdge edge);
    }
}



