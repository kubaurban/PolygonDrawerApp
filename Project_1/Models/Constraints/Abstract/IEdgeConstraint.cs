using Project_1.Models.Shapes.Abstract;

namespace Project_1.Models.Constraints.Abstract
{
    public interface IEdgeConstraint<T>
    {
        IEdge Edge { get; }
        T Value { get; set; }
    }
}
