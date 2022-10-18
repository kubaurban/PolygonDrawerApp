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
        List<Point> GetAllPolygonPoints();
        List<Edge> GetAllPolygonEdges();
        Polygon AddPolygon(List<Point> vertices);
        bool RemovePolygon(Polygon polygon);

        // solitary points management
        List<Point> GetSolitaryPoints();
        Point AddSolitaryPoint(System.Drawing.PointF point);
        void ClearSolitaryPoints();
    }
}
