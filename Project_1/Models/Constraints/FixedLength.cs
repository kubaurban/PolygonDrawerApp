using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;

namespace Project_1.Models.Constraints
{
    public class FixedLength : IEdgeConstraint<float>
    {
        public IEdge Edge { get; private set; }
        public float Value { get; set; }

        public FixedLength(IEdge edge, float length)
        {
            Edge = edge;
            Value = length;
        }
    }
}
