using Project_1.Models.Relations;
using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IRelationRepository
    {
        List<Relation> GetAll();
        Relation GetRelationById(int id);
        Relation AddPerpendicularRelation(Edge edge1, Edge edge2);
        Relation AddFixedEdgeRelation(Edge edge, int length);
        Relation Remove(int id);
        bool Remove(Relation relation);
    }
}
