using Project_1.Models.Constraints;
using Project_1.Models.Repositories.Abstract;
using Project_1.Models.Shapes.Abstract;

namespace Project_1.Models.Repositories
{
    public class ConstraintRepositories : IConstraintRepositories
    {
        public IEdgeConstraintRepository<FixedLength, int> FixedLengthRepository { get; }
        public IEdgeConstraintRepository<Perpendicular, IEdge> PerpendicularRepository { get; }

        public ConstraintRepositories(
            IEdgeConstraintRepository<FixedLength, int> fixedLengthRepository,
            IEdgeConstraintRepository<Perpendicular, IEdge> perpendicularRepository)
        {
            FixedLengthRepository = fixedLengthRepository;
            PerpendicularRepository = perpendicularRepository;
        }

        public void RemoveAllForEdge(IEdge edge)
        {
            FixedLengthRepository.RemoveForEdge(edge);
            PerpendicularRepository.RemoveForEdge(edge);
        }

        public bool HasAnyConstraint(IEdge edge)
        {
            if (FixedLengthRepository.HasConstraint(edge) || PerpendicularRepository.HasConstraint(edge))
                return true;
            return false;
        }
    }
}
