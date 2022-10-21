using Project_1.Helpers.BL;
using Project_1.Models.Constraints;
using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class ConstraintRepository : IConstraintRepository
    {
        private List<Perpendicular> PerpendicularConstraints { get; set; }
        private List<FixedLength> FixedLengthConstraints { get; set; }

        public ConstraintRepository()
        {
            FixedLengthConstraints = new List<FixedLength>();
            PerpendicularConstraints = new List<Perpendicular>();
        }

        public Perpendicular AddPerpendicular(Edge constrained, Edge constraint)
        {
            var newConstraint = new Perpendicular(constrained, constraint);

            PerpendicularConstraints.Add(newConstraint);
            return newConstraint;
        }
        public FixedLength AddFixedLength(Edge edge, int length)
        {
            var newConstraint = new FixedLength(edge, length);

            FixedLengthConstraints.Add(newConstraint);
            return newConstraint;
        }

        public List<FixedLength> GetFixedLengths() => FixedLengthConstraints.ToList();
        public List<Perpendicular> GetPerpendiculars() => PerpendicularConstraints.ToList();

        public FixedLength RemoveFixedLength(FixedLength relation)
        {
            relation.Edge.FixedLength = null;
            FixedLengthConstraints.Remove(relation);
            return relation;
        }
        public Perpendicular RemovePerpendicular(Perpendicular relation)
        {
            relation.Edge.Perpendiculars.Remove(relation);
            PerpendicularConstraints.Remove(relation);
            return relation;
        }
    }
}
