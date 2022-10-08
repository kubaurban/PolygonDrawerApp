using Project_1.Models.Shapes;
using System.Collections.Generic;

namespace Project_1.Models.Repositories
{
    public interface IShapeRepository
    {
        List<Polygon> GetAllPolygons();
        Polygon GetPolygonById(int id);
        Polygon AddPolygon(Polygon polygon);
        Polygon RemovePolygon(int id);
        bool RemovePolygon(Polygon polygon);

        List<Point> GetSolitaryPoints();
        Point AddSolitaryPoint(Point point);
        void ClearSolitaryPoints();
    }
}
