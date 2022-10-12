using Project_1.Models.Relations;
using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;

namespace Project_1.Models.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private static int RelationIdCounter { get; set; }
        private List<Relation> Relations { get; set; }

        public RelationRepository()
        {
            Relations = new List<Relation>();
            RelationIdCounter = 0;
        }

        public Relation AddPerpendicularRelation(Edge edge1, Edge edge2)
        {
            var newRelation = new PerpendicularRelation(++RelationIdCounter)
            {
                FirstEdge = edge1,
                SecondEdge = edge2,
            };
            edge1.AddRelation(RelationIdCounter);
            edge2.AddRelation(RelationIdCounter);

            Relations.Add(newRelation);
            return newRelation;
        }

        public Relation AddFixedEdgeRelation(Edge edge, int length)
        {
            var newRelation = new FixedEdgeLength(++RelationIdCounter)
            {
                FirstEdge = edge,
                Length = length,
            };
            edge.AddRelation(RelationIdCounter);

            Relations.Add(newRelation);
            return newRelation;
        }

        public List<Relation> GetAll() => Relations.ToList();

        public Relation GetRelationById(int id) => Relations.Find(x => x.Id == id);

        public Relation Remove(int id)
        {
            var relation = GetRelationById(id);
            Remove(relation);

            return relation;
        }

        public bool Remove(Relation relation) => Relations.Remove(relation);
    }
}
