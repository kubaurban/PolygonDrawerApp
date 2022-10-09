using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class ShapeRepository : IShapeRepository
    {
        private static int PolygonIdCounter { get; set; }
        private static int PointIdCounter { get; set; }

        private List<Polygon> Polygons { get; set; }
        private ObservableCollection<Point> SolitaryPoints { get; set; }
        public NotifyCollectionChangedEventHandler OnSolitaryPointAdded { get; set; }

        public ShapeRepository()
        {
            Polygons = new List<Polygon>();
            PolygonIdCounter = 0;
            SolitaryPoints = new ObservableCollection<Point>();
            PointIdCounter = 0;
            SolitaryPoints.CollectionChanged += OnSolitaryPointsChanged;
        }

        public Polygon AddPolygon(List<Point> vertices)
        {
            var newPolygon = new Polygon(++PolygonIdCounter)
            {
                Vertices = vertices
            };
            Polygons.Add(newPolygon);
            return newPolygon;
        }

        public Point AddSolitaryPoint(System.Drawing.PointF point)
        {
            var newPoint = new Point(++PointIdCounter)
            {
                X = point.X,
                Y = point.Y
            };
            SolitaryPoints.Add(newPoint);
            return newPoint;
        }

        public void ClearSolitaryPoints() => SolitaryPoints.Clear();

        public List<Polygon> GetAllPolygons() => Polygons.ToList();

        public Polygon GetPolygonById(int id) => Polygons.Find(x => x.Id == id);

        public List<Point> GetSolitaryPoints() => SolitaryPoints.ToList();

        public Polygon RemovePolygon(int id)
        {
            var polygon = GetPolygonById(id);
            RemovePolygon(polygon);

            return polygon;
        }

        public bool RemovePolygon(Polygon polygon) => Polygons.Remove(polygon);

        private void OnSolitaryPointsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnSolitaryPointAdded?.Invoke(sender, e);
            }
        }

        public List<Point> GetAllPolygonPoints() => Polygons.SelectMany(x => x.Vertices).ToList();
        public List<Edge> GetAllPolygonEdges() => Polygons.SelectMany(x => x.Edges).ToList();
    }
}
