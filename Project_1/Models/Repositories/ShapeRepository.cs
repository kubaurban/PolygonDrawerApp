using Project_1.Models.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Project_1.Models.Repositories
{
    public class ShapeRepository : IShapeRepository
    {
        private HashSet<IPolygon> Polygons { get; set; }
        private ObservableCollection<IPoint> SolitaryPoints { get; set; }
        public NotifyCollectionChangedEventHandler OnSolitaryPointAdded { get; set; }

        public ShapeRepository()
        {
            Polygons = new HashSet<IPolygon>();
            SolitaryPoints = new ObservableCollection<IPoint>();
            SolitaryPoints.CollectionChanged += OnSolitaryPointsChanged;
        }

        public Polygon AddPolygon(IList<IPoint> vertices)
        {
            var newPolygon = new Polygon(vertices);
            Polygons.Add(newPolygon);
            return newPolygon;
        }

        #region Polygons
        public bool RemovePolygon(IPolygon polygon) => Polygons.Remove(polygon);

        public List<IPolygon> GetAllPolygons() => Polygons.ToList();
        #endregion

        #region Polygons helpers
        public List<IEdge> GetAllPolygonEdges() => Polygons.SelectMany(x => x.Edges).ToList();

        public List<IPoint> GetAllPolygonPoints() => Polygons.SelectMany(x => x.Vertices).ToList();

        public List<IEdge> GetEdgesByPoint(IPoint u) => GetPolygonByPoint(u).GetNeighborEdges(u);

        public IPolygon GetPolygonByPoint(IPoint u) => Polygons.SingleOrDefault(x => x.Vertices.Contains(u)); 

        public IPolygon GetPolygonByEdge(IEdge u) => Polygons.SingleOrDefault(x => x.Edges.Contains(u)); 
        #endregion

        #region Solitary points
        public Point AddSolitaryPoint(System.Drawing.PointF point)
        {
            var newPoint = new Point()
            {
                X = point.X,
                Y = point.Y
            };
            SolitaryPoints.Add(newPoint);
            return newPoint;
        }

        public List<IPoint> GetSolitaryPoints() => SolitaryPoints.ToList();

        public void ClearSolitaryPoints() => SolitaryPoints.Clear();

        private void OnSolitaryPointsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnSolitaryPointAdded?.Invoke(sender, e);
            }
        }
        #endregion
    }
}
