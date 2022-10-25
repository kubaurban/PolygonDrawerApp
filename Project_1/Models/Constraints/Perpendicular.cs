using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;

namespace Project_1.Models.Constraints
{
    public class Perpendicular : IEdgeConstraint<IEdge>
    {
        private static int _counter = 0;
        public int Id { get; private set; }
        public IEdge Edge { get; private set; }
        public IEdge Value { get; set; }

        public Perpendicular(IEdge constrained, IEdge constraint)
        {
            Id = ++_counter;
            Edge = constrained;
            Value = constraint;
        }

        public override string ToString() => "Perpendicular";
    }
}
