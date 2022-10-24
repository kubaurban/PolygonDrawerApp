using Project_1.Models.Constraints;
using Project_1.Models.Repositories.Abstract;
using Project_1.Models.Shapes.Abstract;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class PerpendicularRepository : IEdgeConstraintRepository<Perpendicular, IEdge>
    {
        private readonly HashSet<Perpendicular> _perpendiculars;

        public PerpendicularRepository()
        {
            _perpendiculars = new HashSet<Perpendicular>();
        }

        public Perpendicular Add(IEdge edge, IEdge value)
        {
            var newConstraint = new Perpendicular(edge, value);
            _perpendiculars.Add(newConstraint);
            return newConstraint;
        }

        public IEnumerable<Perpendicular> Get() => _perpendiculars.AsEnumerable();

        public IEnumerable<Perpendicular> GetForEdge(IEdge edge) => _perpendiculars.Where(x => x.Edge == edge || x.Value == edge);

        public Perpendicular Remove(Perpendicular constraint)
        {
            _perpendiculars.Remove(constraint);
            return constraint;
        }

        public void RemoveForEdge(IEdge edge) => _perpendiculars.RemoveWhere(x => x.Edge == edge || x.Value == edge);

        public bool HasConstraint(IEdge edge) => _perpendiculars.Any(x => x.Edge == edge || x.Value == edge);
    }
}
