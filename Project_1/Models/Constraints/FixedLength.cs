using Project_1.Models.Shapes;

namespace Project_1.Models.Constraints
{
    public class FixedLength : EdgeConstraint<int>
    {
        public FixedLength(Edge edge, int length) : base(edge, length) { }
    }
}
