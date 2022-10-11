using Project_1.Models.Shapes;

namespace Project_1.Models.Relations
{
    public abstract class Relation
    {
        public int Id { get; set; }
        public Edge FirstEdge { get; set; }

        protected Relation(int id)
        {
            Id = id;
        }
    }
}
