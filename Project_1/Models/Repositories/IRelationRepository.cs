using Project_1.Models.Relations;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IRelationRepository
    {
        List<Relation> GetAll();
        Relation GetRelationById(int id);
        Relation Add(Relation relation);
        Relation Remove(int id);
        bool Remove(Relation relation);
    }
}
