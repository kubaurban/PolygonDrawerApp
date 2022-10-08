using Project_1.Models.Relations;
using System.Collections.Generic;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private List<Relation> Relations { get; set; }

        public RelationRepository()
        {
            Relations = new List<Relation>();
        }

        public Relation Add(Relation relation)
        {
            Relations.Add(relation);
            return relation;
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
