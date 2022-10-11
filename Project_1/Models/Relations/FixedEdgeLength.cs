namespace Project_1.Models.Relations
{
    public class FixedEdgeLength : Relation
    {
        public int Length { get; set; }

        public FixedEdgeLength(int id) : base(id) { }
    }
}
