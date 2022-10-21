using Project_1.Helpers.BL;
using Project_1.Helpers.UI;
using Project_1.Models.Constraints;
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
        private readonly IConstraintRepository _constraints;

        public IDrawer Drawer { get => _drawer; }
        public IShapeRepository Shapes { get => _shapes; }
        public IConstraintRepository Constraints { get => _constraints; }

        public Action RedrawAll { get; set; }

        private System.Drawing.Point ClickedPoint { get; set; }
        private IShape MovedShape { get; set; }
        private IEdge SelectedEdge { get; set; }

        public Canvas(IDrawer drawer, IShapeRepository shapes, IConstraintRepository relations)
        {
            _drawer = drawer;
            _shapes = shapes;
            _constraints = relations;

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
            IPoint selectedVertex = default;

            switch (Drawer.Mode)
            {
                case DrawerMode.Draw:
                    selectedVertex = Shapes.GetSolitaryPoints().Find(x => x.WasClicked(ClickedPoint, DrawerClass.PointWidth));

                    if (selectedVertex == default(IPoint))
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
                    selectedVertex = Shapes.GetAllPolygonPoints().Find(x => x.WasClicked(ClickedPoint, DrawerClass.PointWidth));

                    if (selectedVertex != default(IPoint))
                    {
                        var polygon = selectedVertex.Polygon;

                        if (polygon.Vertices.Count > 3)
                        {
                            selectedVertex.GetEdges().Select(x => x.FixedLength).Where(x => x != null).ToList().ForEach(x =>
                            {
                                Constraints.RemoveFixedLength(x);
                            });
                            polygon.RemoveVertex(selectedVertex);
                        }
                        else
                        {
                            polygon.Edges.Select(x => x.FixedLength).Where(x => x != null).ToList().ForEach(x =>
                            {
                                Constraints.RemoveFixedLength(x);
                            });
                            Shapes.RemovePolygon(polygon);
                        }

                        RedrawAll?.Invoke();
                    }
                    break;

                case DrawerMode.Move:
                    #region Vertex selection

                    MovedShape = Shapes.GetAllPolygonPoints().Find(x => x.WasClicked(ClickedPoint, DrawerClass.PointWidth));

                    if (MovedShape != default(Point))
                    {
                        return;
                    }

                    #endregion

                    #region Polygon selection

                    var allPolygons = Shapes.GetAllPolygons().ToList();
                    MovedShape = allPolygons.Find(x => x.WasClicked(ClickedPoint, DrawerClass.MoveIconWidth));

                    if (MovedShape != default(Polygon))
                    {
                        return;
                    }

                    #endregion

                    #region Edge selection

                    var allEdges = Shapes.GetAllPolygonEdges().ToList();
                    MovedShape = allEdges.Find(x => x.WasClicked(ClickedPoint, DrawerClass.PointWidth));

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
            SelectedEdge = allEdges.Find(x => x.WasClicked(ClickedPoint, DrawerClass.PointWidth));

            if (SelectedEdge != default(IEdge))
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

                if (MovedShape is Point point)
                {
                    PointMoveWithConstraints(point, vector);
                }
                else if (MovedShape is Edge edge)
                {
                    EdgeMoveWithConstraints(edge, vector);
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
                    Drawer.DrawLine(solitaryPoints.Last().Center, e.Location);
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

        private void HandleEdgePointInsert(object sender, EventArgs e)
        {
            var point = Shapes.AddSolitaryPoint(new((SelectedEdge.U.X + SelectedEdge.V.X) / 2, (SelectedEdge.U.Y + SelectedEdge.V.Y) / 2));
            var constraint = SelectedEdge.FixedLength;
            if (constraint != default(FixedLength))
            {
                Constraints.RemoveFixedLength(constraint); 
            }
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
                Drawer.DrawLine(lastTwoVerices.First().Center, lastTwoVerices.Last().Center);
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
                Drawer.DrawPoint(prev.Center);
                foreach (var p in solitaryPoints.Skip(1))
                {
                    Drawer.DrawPoint(p.Center);
                    Drawer.DrawLine(prev.Center, p.Center);
                    prev = p;
                }
            }
        }
        #endregion


        public static void PointMoveWithConstraints(Point root, Vector2 rootMove)
        {
            var polygon = root.Polygon;
            var canBeProcessed = polygon.Vertices.ToHashSet();
            var toBeProcessed = new Queue<(Point, Vector2)>();

            toBeProcessed.Enqueue((root, rootMove));
            while (toBeProcessed.Any())
            {
                (var u, var move) = toBeProcessed.Dequeue();

                foreach (var e in u.GetEdges())
                {
                    var v = u.GetNeighbor(e);
                    if (e.FixedLength != null && canBeProcessed.Contains(v))
                    {
                        var vu = new Vector2(u.X - v.X, u.Y - v.Y);
                        var vu_moved = vu + move;
                        var vMove = vu_moved - vu_moved * vu.Length() / vu_moved.Length();
                        toBeProcessed.Enqueue((v, vMove));
                    }
                }

                if (canBeProcessed.Remove(u))
                {
                    u.Move(move); 
                }
            }

            (var last, var moveLast) = toBeProcessed.Dequeue();

            if (allFixed)
            {
                var edges = Shapes.GetEdgesFor(last);
                var u = last.GetNeighbor(edges.First());
                var v = last.GetNeighbor(edges.Last());
                var uv = v - u;
                var uLast = last - u;

                var x = (float)(uv.LengthSquared() + Math.Pow(edges.First().FixedLength.Value, 2) - Math.Pow(edges.Last().FixedLength.Value, 2)) / (2 * uv.Length());

                var t = uLast - uv * Vector2.Dot(uLast, uv) / Vector2.Dot(-uv, -uv);

                var H = Vector2.Normalize(t) * (float)Math.Sqrt(Math.Pow(edges.First().FixedLength.Value, 2) - Math.Pow(x, 2));

                var X = Vector2.Normalize(uv) * x;

                last.Move(-uLast + X + H);
                if (last.X.Equals(float.NaN) || last.Y.Equals(float.NaN))
                {
                    return false;
                }
            }
            else
            {
                last.Move(moveLast);
            }
        }

        private static void EdgeMoveWithConstraints(Edge root, Vector2 rootMove)
        {
            var rootU = root.U;
            var rootV = root.V;

            var canBeProcessed = rootU.Polygon.Vertices.ToHashSet();
            var toBeProcessed = new Queue<(Point, Vector2)>();

            toBeProcessed.Enqueue((rootU, rootMove));
            toBeProcessed.Enqueue((rootV, rootMove));
            while (toBeProcessed.Any())
            {
                (var u, var move) = toBeProcessed.Dequeue();

                foreach (var e in u.GetEdges())
                {
                    var v = u.GetNeighbor(e);
                    if (e.FixedLength != null && canBeProcessed.Contains(v))
                    {
                        var vu = new Vector2(u.X - v.X, u.Y - v.Y);
                        var vu_moved = vu + move;
                        var vMove = vu_moved - vu_moved * vu.Length() / vu_moved.Length();
                        toBeProcessed.Enqueue((v, vMove));
                    }
                }

                if (canBeProcessed.Remove(u))
                {
                    u.Move(move);
                }
            }
        }

        private void SetSelectedEdgeLength(int length)
        {
            var u = SelectedEdge.U;
            var v = SelectedEdge.V;

            var relation = SelectedEdge.FixedLength;
            if (relation != null)
            {
                Constraints.RemoveFixedLength(relation);
            }

            var uv = new Vector2(v.X - u.X, v.Y - u.Y);
            var newVector = uv * length / uv.Length();
            //PointMoveWithConstraints(u, uv - newVector);

            Constraints.AddFixedLength(SelectedEdge, length);
            RedrawAll?.Invoke();
        }
    }
}
