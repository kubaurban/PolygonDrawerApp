using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Project_1.Models.Repositories
{
    public interface IShapeRepository
    {
        // shapes events
        NotifyCollectionChangedEventHandler OnSolitaryPointAdded { get; set; }

        // polygons management
        Polygon AddPolygon(IList<IPoint> vertices);
        bool RemovePolygon(IPolygon polygon);
        List<IPolygon> GetAllPolygons();

        // polygons helpers
        List<IPoint> GetAllPolygonPoints();
        List<IEdge> GetAllPolygonEdges();

        // solitary points management
        List<IPoint> GetSolitaryPoints();
        Point AddSolitaryPoint(System.Drawing.PointF point);
        void ClearSolitaryPoints();
    }
}
