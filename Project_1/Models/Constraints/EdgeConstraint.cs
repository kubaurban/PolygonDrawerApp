using Project_1.Models.Shapes;

namespace Project_1.Models.Constraints
{
    public class EdgeConstraint<T>
    {
        public Edge Edge { get; set; }
        public T Value { get; set; }

        protected EdgeConstraint(Edge edge, T value)
        {
            Edge = edge;
            Value = value;
        }
    }
}
