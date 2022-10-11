using Project_1.Models.Shapes;

namespace Project_1.Models.Relations
{
    public class PerpendicularRelation : Relation
    {
        public Edge SecondEdge { get; set; }

        public PerpendicularRelation(int id) : base(id) { }
    }
}
