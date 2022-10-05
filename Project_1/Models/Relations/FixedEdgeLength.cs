using Project_1.Models.Shapes;

namespace Project_1.Models.Relations
{
    public class FixedEdgeLength : Relation
    {
        public Edge Edge { get; set; }
        public int Length { get; set; }
    }
}
