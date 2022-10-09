using Project_1.Models.Repositories;
using Project_1.Models.Shapes;
using Project_1.Views;
using System.Linq;
using System.Collections.Specialized;
using System.Windows.Forms;
using DrawerClass = Project_1.Views.Drawer;
using System;

namespace Project_1.Presenters
{
    public class Canvas
    {
        private readonly IDrawer _drawer;
        private readonly IShapeRepository _shapes;
        private readonly IRelationRepository _relations;

        public IDrawer Drawer { get => _drawer; }
        public IShapeRepository Shapes { get => _shapes; }
        public IRelationRepository Relations { get => _relations; }

        public Action RedrawAll { get; set; }

        private System.Drawing.Point ClickedPoint { get; set; }
        private Shape MovedShape { get; set; }
        private Edge SelectedEdge { get; set; }

        public Canvas(IDrawer drawer, IShapeRepository shapes, IRelationRepository relations)
        {
            _drawer = drawer;
            _shapes = shapes;
            _relations = relations;
            
            RedrawAll += Drawer.ClearArea;
            RedrawAll += DrawAllPolygons;
            RedrawAll += Drawer.RefreshArea;

            InitModelChangedHandlers();
            InitActionHandlers();
            InitBusinessLogicHandlers();
        }

        private void HandleModeChanged(object sender, EventArgs e)
        {
            RedrawAll?.Invoke();
        }

        public void InitActionHandlers()
        {
            Drawer.LeftMouseDownHandler += HandleLeftMouseDownEvent;
            Drawer.LeftMouseUpHandler += HandleLeftMouseUpEvent;
            Drawer.RightMouseDownHandler += HandleRightMouseDownEvent;
            Drawer.MouseDownMoveHandler += HandleMouseDownMoveEvent;
        }

        public void InitBusinessLogicHandlers()
        {
            Drawer.ModeChangedHandler += HandleModeChanged;
            Drawer.EdgeInsertPointClickedHandler += OnEdgeInsertPointClicked;
            Drawer.EdgeSetFixedLengthClickedHandler += OnEdgeSetFixedLengthClicked;
        }

        public void InitModelChangedHandlers()
        {
            Shapes.OnSolitaryPointAdded += HandleSolitaryPointAdded;
        }

        public void HandleLeftMouseDownEvent(object sender, MouseEventArgs e)
        {
            ClickedPoint = e.Location;
            Point selectedVertex = default;

            if (Drawer.IsInDrawingMode)
            {
                selectedVertex = Shapes.GetSolitaryPoints().Find(x => DrawerClass.IsInsidePoint(ClickedPoint, x, DrawerClass.PointWidth));

                if (selectedVertex == default(Point))
                {
                    Drawer.DrawPoint(ClickedPoint);
                    Shapes.AddSolitaryPoint(ClickedPoint);
                    Drawer.RefreshArea();
                }
                else if (selectedVertex == Shapes.GetSolitaryPoints().First() && Shapes.GetSolitaryPoints().Count > 2)
                {
                    Shapes.AddPolygon(Shapes.GetSolitaryPoints());

                    Shapes.ClearSolitaryPoints();
                    RedrawAll?.Invoke();
                }
            }
            else if (Drawer.IsInDeleteMode)
            {
                selectedVertex = Shapes.GetAllPolygonPoints().Find(x => DrawerClass.IsInsidePoint(ClickedPoint, x, DrawerClass.PointWidth));

                if (selectedVertex != default(Point))
                {
                    var polygon = Shapes.GetPolygonById(selectedVertex.PolygonId);

                    if (polygon.Vertices.Count > 3)
                    {
                        polygon.RemoveVertex(selectedVertex);
                    }
                    else
                    {
                        Shapes.RemovePolygon(polygon);
                    }

                    RedrawAll?.Invoke();
                }
            }
            else if (Drawer.IsInMoveMode)
            {
                #region Vertex selection

                MovedShape = Shapes.GetAllPolygonPoints().Find(x => DrawerClass.IsInsidePoint(ClickedPoint, x, DrawerClass.PointWidth));

                if (MovedShape != default(Point))
                {
                    return;
                }

                #endregion

                #region Polygon selection

                var allPolygons = Shapes.GetAllPolygons().ToList();
                MovedShape = allPolygons.Find(x => DrawerClass.IsInsidePoint(ClickedPoint, x.GravityCenterPoint, DrawerClass.MoveIconWidth));

                if (MovedShape != default(Polygon))
                {
                    return;
                }

                #endregion

                #region Edge selection

                var allEdges = Shapes.GetAllPolygonEdges().ToList();
                MovedShape = allEdges.Find(x => DrawerClass.IsInsideEdge(ClickedPoint, x));

                if (MovedShape != default(Edge))
                {
                    return;
                }

                #endregion
            }
        }

        public void HandleLeftMouseUpEvent(object sender, MouseEventArgs e)
        {
            MovedShape = default;
        }

        public void HandleRightMouseDownEvent(object sender, MouseEventArgs e)
        {
            ClickedPoint = e.Location;

            var allEdges = Shapes.GetAllPolygonEdges().ToList();
            SelectedEdge = allEdges.Find(x => DrawerClass.IsInsideEdge(ClickedPoint, x));

            if (SelectedEdge != default(Edge))
            {
                Drawer.ShowManageEdgeMenu(ClickedPoint);
            }
        }

        public void HandleMouseDownMoveEvent(object sender, MouseEventArgs e)
        {
            var vector = new System.Drawing.Point
            {
                X = e.Location.X - ClickedPoint.X,
                Y = e.Location.Y - ClickedPoint.Y
            };
            ClickedPoint = e.Location;

            if (Drawer.IsInMoveMode && MovedShape != default(Shape))
            {
                MovedShape.Move(vector);
                RedrawAll?.Invoke();
            }
        }

        private void OnEdgeSetFixedLengthClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnEdgeInsertPointClicked(object sender, EventArgs e)
        {
            var polygon = Shapes.GetAllPolygons().Find(x => x.Id == SelectedEdge.PolygonId);

            var point = Shapes.AddSolitaryPoint(new((int)(SelectedEdge.U.X + SelectedEdge.V.X) / 2, (int)(SelectedEdge.U.Y + SelectedEdge.V.Y) / 2));
            polygon.InsertPoint(SelectedEdge, point);
            Shapes.ClearSolitaryPoints();

            RedrawAll?.Invoke();
        }

        private void HandleSolitaryPointAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            var solitaryPointsCount = Shapes.GetSolitaryPoints().Count;
            if (solitaryPointsCount > 1)
            {
                var lastTwoVerices = Shapes.GetSolitaryPoints().Skip(solitaryPointsCount - 2);
                Drawer.DrawLine(lastTwoVerices.First(), lastTwoVerices.Last());
            }
        }

        private void DrawAllPolygons()
        {
            Drawer.DrawPolygons(Shapes.GetAllPolygons());
        }
    }
}
