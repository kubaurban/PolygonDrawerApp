using Project_1.Models.Shapes;

namespace Project_1.Models.Constraints
{
    public interface IEdgeConstraint<T>
    {
        IEdge Edge { get; }
        T Value { get; set; }
    }
}
