using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;
using System.Collections.Generic;

namespace Project_1.Models.Repositories.Abstract
{
    public interface IEdgeConstraintRepository<T, U> where T : IEdgeConstraint<U>
    {
        T Add(IEdge edge, U value);

        T Remove(T constraint);
        void RemoveForEdge(IEdge edge);

        IEnumerable<T> Get();
        IEnumerable<T> GetForEdge(IEdge edge);

        bool HasConstraint(IEdge edge);
    }
}
