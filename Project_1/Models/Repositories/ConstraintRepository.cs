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
            constrained.Perpendiculars.Add(newConstraint);

            PerpendicularConstraints.Add(newConstraint);
            return newConstraint;
        }
        public FixedLength AddFixedLength(Edge edge, int length)
        {
            var newConstraint = new FixedLength(edge, length);
            edge.FixedLength = newConstraint;

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

        public static List<Point> GetFixedLengthRelated(Point u)
        {
            var fixedLengths = u.Polygon.Edges.Where(x => x.U == u || x.V == u).Select(x => x.FixedLength).Where(x => x != null).ToList();

            var neighbors = new List<Point>();
            fixedLengths.ForEach(x =>
            {
                if (x.Edge.U != u)
                {
                    neighbors.Add(x.Edge.U);
                }
                else
                {
                    neighbors.Add(x.Edge.V);
                }
            });

            return neighbors;
        }
    }
}
