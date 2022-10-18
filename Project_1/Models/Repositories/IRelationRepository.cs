using Project_1.Models.Relations;
using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IRelationRepository
    {
        FixedEdgeLength AddFixedEdgeRelation(Edge edge, int length);
        PerpendicularRelation AddPerpendicularRelation(Edge edge1, Edge edge2);
        List<FixedEdgeLength> GetFixedRelations();
        List<PerpendicularRelation> GetPerpendicularRelations();
        FixedEdgeLength GetFixedEdgeLengthRelationById(int id);
        PerpendicularRelation GetPerpendicularRelationById(int id);
        FixedEdgeLength RemoveFixedEdgeLength(int id);
        PerpendicularRelation RemovePerpendicularRelation(int id);
    }
}
