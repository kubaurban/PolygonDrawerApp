using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;

namespace Project_1.Models.Constraints
{
    public class FixedLength : IEdgeConstraint<int>
    {
        public IEdge Edge { get; private set; }
        public int Value { get; set; }

        public FixedLength(IEdge edge, int length)
        {
            Edge = edge;
            Value = length;
        }
    }
}
