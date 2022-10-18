using Project_1.Helpers.UI;
using Project_1.Models.Relations;
using Project_1.Models.Repositories;
using Project_1.Models.Shapes;
using Project_1.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using DrawerClass = Project_1.Views.Drawer;

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

        #region Init handler functions
        public void InitActionHandlers()
        {
            Drawer.LeftMouseDownHandler += HandleLeftMouseDown;
            Drawer.LeftMouseUpHandler += HandleLeftMouseUp;
            Drawer.RightMouseDownHandler += HandleRightMouseDown;
            Drawer.MouseDownMoveHandler += HandleMouseDownMove;
            Drawer.MouseUpMoveHandler += HandleMouseUpMove;
        }

        public void InitBusinessLogicHandlers()
        {
            Drawer.ModeChangedHandler += HandleModeChange;
            Drawer.EdgeInsertPointClickedHandler += HandleEdgePointInsert;
            Drawer.EdgeSetLengthClickedHandler += HandleEdgeSetFixedLength;
        }

        public void InitModelChangedHandlers()
        {
            Shapes.OnSolitaryPointAdded += HandleSolitaryPointAdd;
        }
        #endregion

        #region Handlers
        private void HandleModeChange(object sender, EventArgs e)
        {
            ClickedPoint = default;
            MovedShape = default;
            SelectedEdge = default;
            RedrawAll?.Invoke();
        }

        public void HandleLeftMouseDown(object sender, MouseEventArgs e)
        {
            ClickedPoint = e.Location;
            Point selectedVertex = default;

            switch (Drawer.Mode)
            {
                case DrawerMode.Draw:
                    selectedVertex = Shapes.GetSolitaryPoints().Find(x => DrawerClass.IsInsidePoint(ClickedPoint, x.Location, DrawerClass.PointWidth));

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
                    break;

                case DrawerMode.Delete:
                    selectedVertex = Shapes.GetAllPolygonPoints().Find(x => DrawerClass.IsInsidePoint(ClickedPoint, x.Location, DrawerClass.PointWidth));

                    if (selectedVertex != default(Point))
                    {
                        var polygon = selectedVertex.Polygon;

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
                    break;

                case DrawerMode.Move:
                    #region Vertex selection

                    MovedShape = Shapes.GetAllPolygonPoints().Find(x => DrawerClass.IsInsidePoint(ClickedPoint, x.Location, DrawerClass.PointWidth));

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

                    #endregion
                    break;

                case DrawerMode.MakePerpendicular:

                    break;
                default:
                    break;
            }
        }

        public void HandleLeftMouseUp(object sender, MouseEventArgs e)
        {
            MovedShape = default;
        }

        public void HandleRightMouseDown(object sender, MouseEventArgs e)
        {
            ClickedPoint = e.Location;

            var allEdges = Shapes.GetAllPolygonEdges().ToList();
            SelectedEdge = allEdges.Find(x => DrawerClass.IsInsideEdge(ClickedPoint, x));

            if (SelectedEdge != default(Edge))
            {
                Drawer.ShowManageEdgeMenu(ClickedPoint);
            }
        }

        public void HandleMouseDownMove(object sender, MouseEventArgs e)
        {
            if (Drawer.Mode == DrawerMode.Move)
            {
                var vector = new Vector2
                {
                    X = e.Location.X - ClickedPoint.X,
                    Y = e.Location.Y - ClickedPoint.Y
                };
                ClickedPoint = e.Location;

                if (MovedShape is Point)
                {
                    MoveWithConstraints(MovedShape as Point, vector);
                }
                else if (MovedShape is Edge)
                {
                    MovedShape.Move(vector);
                }
                else if (MovedShape is Polygon)
                {
                    MovedShape.Move(vector);
                }
                else
                {
                    return;
                }

                RedrawAll?.Invoke();
            }
        }

        private void HandleMouseUpMove(object sender, MouseEventArgs e)
        {
            if (Drawer.Mode == DrawerMode.Draw)
            {
                var solitaryPoints = Shapes.GetSolitaryPoints();
                if (solitaryPoints.Count > 0)
                {
                    Drawer.ClearArea();
                    DrawAllPolygons();
                    DrawAllSolitaryPoints();
                    Drawer.DrawLine(solitaryPoints.Last().Location, e.Location);
                    Drawer.RefreshArea();
                }
            }
        }

        private void HandleEdgeSetFixedLength(object sender, EventArgs e)
        {
            var lengthInputDialog = new LengthInputDialog(SelectedEdge.Length);
            if (lengthInputDialog.ShowDialog() == DialogResult.OK && lengthInputDialog.InputLength > 0)
            {
                SetSelectedEdgeLength(lengthInputDialog.InputLength);
            }
            lengthInputDialog.Close();
            lengthInputDialog.Dispose();
        }

        private void SetSelectedEdgeLength(int length)
        {
            var u = SelectedEdge.U;
            var v = SelectedEdge.V;

            var relation = SelectedEdge.RelationIds.Select(x => Relations.GetFixedEdgeLengthRelationById(x)).SingleOrDefault();
            if (relation != null)
            {
                Relations.RemoveFixedEdgeLength(relation.Id);
            }

            var uv = new Vector2(v.X - u.X, v.Y - u.Y);
            var newVector = uv * length / uv.Length();
            MoveWithConstraints(u, uv - newVector);

            Relations.AddFixedEdgeRelation(SelectedEdge, length);
            RedrawAll?.Invoke();
        }

        private void MoveWithConstraints(Point root, Vector2 rootMove)
        {
            var polygon = root.Polygon;
            var canBeProcessed = polygon.Vertices.ToHashSet();
            var stack = new Stack<(Point, Vector2)>();

            stack.Push((root, rootMove));
            canBeProcessed.Remove(root);
            while (stack.Any())
            {
                (var u, var move) = stack.Pop();

                foreach (var v in GetFixedLengthRelated(u))
                {
                    if (v != null && canBeProcessed.Remove(v))
                    {
                        var vu = new Vector2(u.X - v.X, u.Y - v.Y);
                        var vu_moved = vu + move;
                        var vMove = vu_moved - vu_moved * vu.Length() / vu_moved.Length();
                        stack.Push((v, vMove));
                    }
                }

                u.Move(move);
            }
        }

        private List<Point> GetFixedLengthRelated(Point u)
        {
            var fixedLengthRelations = u.RelationIds.Select(x => Relations.GetFixedEdgeLengthRelationById(x)).ToList();
            
            var neighbors = new List<Point>();
            fixedLengthRelations.ForEach(x =>
            {
                neighbors.Add(x.FirstEdge.U);
                neighbors.Add(x.FirstEdge.V);
            });
            neighbors.RemoveAll(x => x == u);

            return neighbors;
        }

        private void HandleEdgePointInsert(object sender, EventArgs e)
        {
            var point = Shapes.AddSolitaryPoint(new((SelectedEdge.U.X + SelectedEdge.V.X) / 2, (SelectedEdge.U.Y + SelectedEdge.V.Y) / 2));
            SelectedEdge.Polygon.InsertPoint(SelectedEdge, point);
            Shapes.ClearSolitaryPoints();

            RedrawAll?.Invoke();
        }

        private void HandleSolitaryPointAdd(object sender, NotifyCollectionChangedEventArgs e)
        {
            var solitaryPointsCount = Shapes.GetSolitaryPoints().Count;
            if (solitaryPointsCount > 1)
            {
                var lastTwoVerices = Shapes.GetSolitaryPoints().Skip(solitaryPointsCount - 2);
                Drawer.DrawLine(lastTwoVerices.First().Location, lastTwoVerices.Last().Location);
            }
        }
        #endregion

        #region Drawing helpers
        private void DrawAllPolygons()
        {
            Drawer.DrawPolygons(Shapes.GetAllPolygons());
        }

        private void DrawAllSolitaryPoints()
        {
            var solitaryPoints = Shapes.GetSolitaryPoints();

            if (solitaryPoints.Count > 0)
            {
                var prev = solitaryPoints.First();
                Drawer.DrawPoint(prev.Location);
                foreach (var p in solitaryPoints.Skip(1))
                {
                    Drawer.DrawPoint(p.Location);
                    Drawer.DrawLine(prev.Location, p.Location);
                    prev = p;
                }
            }
        }
        #endregion
    }
}
