using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IShapeRepository
    {
        public List<Polygon> GetAllPolygons();
        public Polygon GetPolygonById(int id);
        public Polygon AddPolygon(Polygon polygon);
        public Polygon RemovePolygon(int id);
        public bool RemovePolygon(Polygon polygon);

        public List<Point> GetSolitaryPoints();
        public Point GetSolitaryPointById(int id);
        public Point AddSolitaryPoint(Point point);
        public void ClearSolitaryPoints();
    }
}
