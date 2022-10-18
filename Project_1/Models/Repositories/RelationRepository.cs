using Project_1.Models.Relations;
using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private static int FixedEdgeLengthRelationIdCounter { get; set; }
        private static int PerpendicularRelationIdCounter { get; set; }
        private List<PerpendicularRelation> PerpendicularRelations { get; set; }
        private List<FixedEdgeLength> FixedLengthRelations { get; set; }

        public RelationRepository()
        {
            FixedLengthRelations = new List<FixedEdgeLength>();
            PerpendicularRelations = new List<PerpendicularRelation>();
            FixedEdgeLengthRelationIdCounter = 0;
            PerpendicularRelationIdCounter = 0;
        }

        public PerpendicularRelation AddPerpendicularRelation(Edge edge1, Edge edge2)
        {
            var newRelation = new PerpendicularRelation(++PerpendicularRelationIdCounter)
            {
                FirstEdge = edge1,
                SecondEdge = edge2,
            };
            edge1.AddRelation(PerpendicularRelationIdCounter);
            edge2.AddRelation(PerpendicularRelationIdCounter);

            PerpendicularRelations.Add(newRelation);
            return newRelation;
        }
        public FixedEdgeLength AddFixedEdgeRelation(Edge edge, int length)
        {
            var newRelation = new FixedEdgeLength(++FixedEdgeLengthRelationIdCounter)
            {
                FirstEdge = edge,
                Length = length,
            };
            edge.AddRelation(FixedEdgeLengthRelationIdCounter);

            FixedLengthRelations.Add(newRelation);
            return newRelation;
        }

        public List<FixedEdgeLength> GetFixedRelations() => FixedLengthRelations.ToList();
        public List<PerpendicularRelation> GetPerpendicularRelations() => PerpendicularRelations.ToList();

        public FixedEdgeLength GetFixedEdgeLengthRelationById(int id) => FixedLengthRelations.Find(x => x.Id == id);
        public PerpendicularRelation GetPerpendicularRelationById(int id) => PerpendicularRelations.Find(x => x.Id == id);

        public FixedEdgeLength RemoveFixedEdgeLength(int id)
        {
            var relation = GetFixedEdgeLengthRelationById(id);
            Remove(relation);

            return relation;
        }
        public PerpendicularRelation RemovePerpendicularRelation(int id)
        {
            var relation = GetPerpendicularRelationById(id);
            Remove(relation);

            return relation;
        }

        public bool Remove(FixedEdgeLength relation) => FixedLengthRelations.Remove(relation);
        public bool Remove(PerpendicularRelation relation) => PerpendicularRelations.Remove(relation);
    }
}
