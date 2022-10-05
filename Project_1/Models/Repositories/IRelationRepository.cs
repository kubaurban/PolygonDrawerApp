using Project_1.Models.Relations;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IRelationRepository
    {
        public List<Relation> GetAll();
        public Relation GetRelationById(int id);
        public Relation Add(Relation relation);
        public Relation Remove(int id);
        public bool Remove(Relation relation);
    }
}
