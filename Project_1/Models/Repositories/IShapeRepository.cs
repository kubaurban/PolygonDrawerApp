using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Project_1.Models.Repositories
{
    public interface IShapeRepository
    {
        // shapes events
        public NotifyCollectionChangedEventHandler OnSolitaryPointAdded { get; set; }

        // polygons management
        List<Polygon> GetAllPolygons();
        Polygon GetPolygonById(int id);
        Polygon AddPolygon(Polygon polygon);
        Polygon RemovePolygon(int id);
        bool RemovePolygon(Polygon polygon);

        // solitary points management
        List<Point> GetSolitaryPoints();
        Point AddSolitaryPoint(Point point);
        void ClearSolitaryPoints();
    }
}
