using Project_1.Models.Constraints;
using Project_1.Models.Shapes.Abstract;

namespace Project_1.Models.Repositories.Abstract
{
    public interface IConstraintRepositories
    {
        IEdgeConstraintRepository<FixedLength, int> FixedLengthRepository { get; }
        IEdgeConstraintRepository<Perpendicular, IEdge> PerpendicularRepository { get; }

        void RemoveAllForEdge(IEdge edge);
        bool HasAnyConstraint(IEdge edge);
    }
}



