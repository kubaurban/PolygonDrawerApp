using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class ShapeRepository : IShapeRepository
    {
        private List<Polygon> Polygons { get; set; }
        private List<Point> SolitaryPoints { get; set; }

        public Polygon AddPolygon(Polygon polygon)
        {
            Polygons.Add(polygon);
            return polygon;
        }

        public Point AddSolitaryPoint(Point point)
        {
            SolitaryPoints.Add(point);
            return point;
        }

        public void ClearSolitaryPoints() => SolitaryPoints.Clear();

        public List<Polygon> GetAllPolygons() => Polygons.ToList();

        public Polygon GetPolygonById(int id) => Polygons.Find(x => x.Id == id);

        public Point GetSolitaryPointById(int id) => SolitaryPoints.Find(x => x.Id == id);

        public List<Point> GetSolitaryPoints() => SolitaryPoints.ToList();

        public Polygon RemovePolygon(int id)
        {
            var polygon = GetPolygonById(id);
            RemovePolygon(polygon);

            return polygon;
        }

        public bool RemovePolygon(Polygon polygon) => Polygons.Remove(polygon);
    }
}
