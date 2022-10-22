using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;

namespace Project_1.Models.Constraints
{
    public class Perpendicular : IEdgeConstraint<IEdge>
    {
        public IEdge Edge { get; private set; }
        public IEdge Value { get; set; }

        public Perpendicular(IEdge constrained, IEdge constraint)
        {
            Edge = constrained;
            Value = constraint;
        }
    }
}
