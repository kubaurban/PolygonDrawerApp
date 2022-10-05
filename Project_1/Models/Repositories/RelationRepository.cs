using Project_1.Models.Relations;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private List<Relation> Relations { get; set; }
    }
}
