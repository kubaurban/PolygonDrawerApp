using Project_1.Models.Shapes;

namespace Project_1.Models.Constraints
{
    public class Perpendicular : EdgeConstraint<Edge>
    {
        public Perpendicular(Edge constrained, Edge constraint) : base(constrained, constraint) { }
    }
}
