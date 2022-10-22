using Project_1.Helpers.BL;
using Project_1.Helpers.UI;
using Project_1.Models.Repositories.Abstract;
using Project_1.Models.Shapes;
using Project_1.Models.Shapes.Abstract;
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
        private readonly IConstraintRepositories _constraintsRepositories;

        public IDrawer Drawer { get => _drawer; }
        public IShapeRepository Shapes { get => _shapes; }
        public IConstraintRepositories Constraints { get => _constraintsRepositories; }

        public Action RedrawAll { get; set; }

        private System.Drawing.Point ClickedPoint { get; set; }
        private IShape MovedShape { get; set; }
        private IEdge SelectedEdge { get; set; }

        public Canvas(IDrawer drawer, IShapeRepository shapes, IConstraintRepositories constraintRepositories)
        {
            _drawer = drawer;
            _shapes = shapes;
            _constraintsRepositories = constraintRepositories;

            RedrawAll += Drawer.ClearArea;
            RedrawAll += DrawAllPolygons;
            RedrawAll += WriteEdgesFixedLengths;
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
                        var polygon = Shapes.GetPolygonByPoint(selectedVertex);

                        if (polygon.Edges.Count > 3)
                        {
                            foreach (var edge in Shapes.GetEdgesByPoint(selectedVertex))
                            {
                                Constraints.RemoveAllForEdge(edge);
                            }

                            polygon.RemoveVertex(selectedVertex);
                        }
                        else
                        {
                            foreach (var edge in polygon.Edges)
                            {
                                Constraints.RemoveAllForEdge(edge);
                            }
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

            Constraints.FixedLengthRepository.RemoveForEdge(SelectedEdge);

            Shapes.GetPolygonByEdge(SelectedEdge).InsertPoint(SelectedEdge, point);
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

        public void WriteEdgesFixedLengths()
        {
            foreach (var e in Shapes.GetAllPolygonEdges())
            {
                if (Constraints.FixedLengthRepository.HasConstraint(e))
                {
                    WriteEdgeLength(e);
                }
            }
        }

        private void WriteEdgeLength(IEdge edge)
        {
            Drawer.Write(edge.Center, edge.Length.ToString());
        }
        #endregion


        public void PointMoveWithConstraints(IPoint root, Vector2 rootMove)
        {
            var polygon = Shapes.GetPolygonByPoint(root);

            var verticesCopy = new List<IPoint>();
            foreach (var item in polygon.Vertices)
            {
                verticesCopy.Add(item.Clone() as IPoint);
            }

            var toBeProcessed = new Queue<(IPoint, Vector2)>();
            toBeProcessed.Enqueue((root, rootMove));

            if (!Algorithm(polygon, toBeProcessed))
            {
                // retrieve start vertices coordinates
                var it = verticesCopy.GetEnumerator();
                foreach (var v in polygon.Vertices)
                {
                    it.MoveNext();
                    v.X = it.Current.X;
                    v.Y = it.Current.Y;
                }

                // move entire polygon at last
                polygon.Move(rootMove);
            }
        }

        private bool Algorithm(IPolygon polygon, Queue<(IPoint, Vector2)> toBeProcessed)
        {
            var canBeProcessed = polygon.Vertices.ToHashSet();

            while (toBeProcessed.Any() && canBeProcessed.Count > 0)
            {
                (var u, var move) = toBeProcessed.Dequeue();

                if (canBeProcessed.Remove(u))
                {
                    foreach (var e in polygon.GetNeighborEdges(u))
                    {
                        var v = u.GetNeighbor(e);
                        if (Constraints.FixedLengthRepository.HasConstraint(e) && canBeProcessed.Contains(v))
                        {
                            var vu = new Vector2(u.X - v.X, u.Y - v.Y);
                            var vu_moved = vu + move;
                            var vMove = vu_moved - vu_moved * vu.Length() / vu_moved.Length();
                            toBeProcessed.Enqueue((v, vMove));
                        }
                    }

                    u.Move(move);
                    if (u.X.Equals(float.NaN) || u.Y.Equals(float.NaN))
                    {
                        return false;
                    }
                }
            }

            if (toBeProcessed.Any())
            {
                (var last, _) = toBeProcessed.Dequeue();

                var edges = polygon.GetNeighborEdges(last);

                var e = edges.First();
                var f = edges.Last();

                var u = last.GetNeighbor(e);
                var v = last.GetNeighbor(f);
                var uv = v - u;
                var uLast = last - u;

                var eLengthConstraint = Constraints.FixedLengthRepository.GetForEdge(e).Single().Value;
                var fLengthConstraint = Constraints.FixedLengthRepository.GetForEdge(f).Single().Value;

                var x = (float)(uv.LengthSquared() + Math.Pow(eLengthConstraint, 2) - Math.Pow(fLengthConstraint, 2)) / (2 * uv.Length());

                var t = uLast - uv * Vector2.Dot(uLast, uv) / Vector2.Dot(-uv, -uv);

                var H = Vector2.Normalize(t) * (float)Math.Sqrt(Math.Pow(eLengthConstraint, 2) - Math.Pow(x, 2));

                var X = Vector2.Normalize(uv) * x;

                last.Move(-uLast + X + H);
                if (last.X.Equals(float.NaN) || last.Y.Equals(float.NaN))
                {
                    return false;
                }
            }

            return true;
        }

        private void EdgeMoveWithConstraints(IEdge root, Vector2 rootMove)
        {
            var polygon = Shapes.GetPolygonByEdge(root);

            var verticesCopy = new List<IPoint>();
            foreach (var item in polygon.Vertices)
            {
                verticesCopy.Add(item.Clone() as IPoint);
            }

            var toBeProcessed = new Queue<(IPoint, Vector2)>();
            toBeProcessed.Enqueue((root.U, rootMove));
            toBeProcessed.Enqueue((root.V, rootMove));

            if (!Algorithm(polygon, toBeProcessed))
            {
                // retrieve start vertices coordinates
                var it = verticesCopy.GetEnumerator();
                foreach (var v in polygon.Vertices)
                {
                    it.MoveNext();
                    v.X = it.Current.X;
                    v.Y = it.Current.Y;
                }

                // move entire polygon at last
                polygon.Move(rootMove);
            }
        }

        private void SetSelectedEdgeLength(int length)
        {
            var u = SelectedEdge.U;
            var v = SelectedEdge.V;

            var polygon = Shapes.GetPolygonByEdge(SelectedEdge);
            var otherFixedLengths = polygon.Edges
                .Where(x => x != SelectedEdge)
                .SelectMany(x => Constraints.FixedLengthRepository.GetForEdge(x)).ToList();

            if (otherFixedLengths.Count == polygon.Edges.Count - 1 && otherFixedLengths.Sum(x => x.Value) <= length)
            {
                return;
            }

            Constraints.FixedLengthRepository.RemoveForEdge(SelectedEdge);

            var uv = new Vector2(v.X - u.X, v.Y - u.Y);
            var newVector = uv * length / uv.Length();
            PointMoveWithConstraints(u, uv - newVector);

            Constraints.FixedLengthRepository.Add(SelectedEdge, length);
            RedrawAll?.Invoke();
        }
    }
}
