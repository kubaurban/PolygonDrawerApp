using Project_1.Models.Constraints;
using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class ConstraintRepository : IConstraintRepository
    {
        private ISet<FixedLength> FixedLengthConstraints { get; set; }
        private ISet<Perpendicular> PerpendicularConstraints { get; set; }

        public ConstraintRepository()
        {
            FixedLengthConstraints = new HashSet<FixedLength>();
            PerpendicularConstraints = new HashSet<Perpendicular>();
        }

        public Perpendicular AddPerpendicular(IEdge constrained, IEdge constraint)
        {
            var newConstraint = new Perpendicular(constrained, constraint);
            PerpendicularConstraints.Add(newConstraint);
            return newConstraint;
        }

        public FixedLength AddFixedLength(IEdge edge, int length)
        {
            var newConstraint = new FixedLength(edge, length);
            FixedLengthConstraints.Add(newConstraint);
            return newConstraint;
        }

        public List<FixedLength> GetFixedLengths() => FixedLengthConstraints.ToList();
        public List<Perpendicular> GetPerpendiculars() => PerpendicularConstraints.ToList();

        public FixedLength RemoveFixedLength(FixedLength relation)
        {
            FixedLengthConstraints.Remove(relation);
            return relation;
        }

        public void RemovePerpendiculars(IList<Perpendicular> relations)
        {
            foreach (var rel in relations)
        {
                PerpendicularConstraints.Remove(rel);
            }
        }
    }
}
