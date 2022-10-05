using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public class ShapeRepository : IShapeRepository
    {
        private List<Polygon> Polygons { get; set; }
        private List<Point> Points { get; set; }
    }
}
