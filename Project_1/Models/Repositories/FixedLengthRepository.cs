using Project_1.Models.Constraints;
using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class FixedLengthRepository : IEdgeConstraintRepository<FixedLength, int>
    {
        private readonly HashSet<FixedLength> _fixedLength;

        public FixedLengthRepository()
        {
            _fixedLength = new HashSet<FixedLength>();
        }

        public FixedLength Add(IEdge edge, int length)
        {
            var newConstraint = new FixedLength(edge, length);
            _fixedLength.Add(newConstraint);
            return newConstraint;
        }

        public IEnumerable<FixedLength> Get() => _fixedLength.AsEnumerable();

        public IEnumerable<FixedLength> GetForEdge(IEdge edge)
        {
            var fixedLength = _fixedLength.SingleOrDefault(x => x.Edge == edge);
            if (fixedLength is not null)
                yield return fixedLength;
            yield break;
        }

        public bool HasConstraint(IEdge edge) => _fixedLength.Any(x => x.Edge == edge);

        public FixedLength Remove(FixedLength constraint)
        {
            _fixedLength.Remove(constraint);
            return constraint;
        }

        public void RemoveForEdge(IEdge edge)
        {
            _fixedLength.RemoveWhere(x => x.Edge == edge);
        }
    }
}
