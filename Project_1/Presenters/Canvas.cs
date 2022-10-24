using Project_1.Helpers.BL;
using Project_1.Helpers.UI;
using Project_1.Models.Constraints;
using Project_1.Models.Constraints.Abstract;
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
        #region Injected components
        private IDrawer Drawer { get; }
        private IShapeRepository Shapes { get; }
        private IConstraintRepositories Constraints { get; }
        #endregion

        #region Settings
        public System.Drawing.Color SpecialColor { get; }
        #endregion

        private Action RedrawAll { get; set; }

        private System.Drawing.Point Click { get; set; }

        private IEdge _selectedEdge;
        private IEdge SelectedEdge
        {
            get => _selectedEdge;
            set
            {
                _selectedEdge = value;

                Drawer.UnsetSelectedRelation();
                if (value == null)
                {
                    Drawer.DisableRelationsBoxVisibility();
                }
                else
                {
                    RefreshRelationsList();
                    Drawer.UnsetSelectedRelation();
                    Drawer.EnableRelationsBoxVisibility();
                }
                RedrawAll?.Invoke();
                Drawer.RefreshArea();
            }
        }
        private IMovable MovingItem { get; set; }
        private IEdge MakePerpendicularEdge { get; set; }
        private IEdgeConstraint<IEdge> SelectedRelation => Drawer.GetSelectedRelation();
        private HashSet<Perpendicular> QueuedPerpendiculars { get; }

        public Canvas(IDrawer drawer, IShapeRepository shapes, IConstraintRepositories constraintRepositories)
        {
            Drawer = drawer;
            Shapes = shapes;
            Constraints = constraintRepositories;

            _selectedEdge = null;
            QueuedPerpendiculars = new HashSet<Perpendicular>();

            #region Settings
            SpecialColor = System.Drawing.Color.BlueViolet;
            #endregion

            #region RedrawAll delegate
            RedrawAll += Drawer.ClearArea;
            RedrawAll += DrawAllPolygons;
            RedrawAll += DrawAllSolitaryPoints;
            RedrawAll += WriteEdgesFixedLengths;
            RedrawAll += RecolorSelectedRelationEdges;
            #endregion

            #region Assign move with constraints functions to shapes
            Point.ConstrainedMove += PointMoveWithConstraints;
            Edge.ConstrainedMove += EdgeMoveWithConstraints;
            #endregion

            InitModelChangedHandlers();
            InitActionHandlers();
            InitBusinessLogicHandlers();

            InitDefaultScene();
        }


        private void InitDefaultScene()
        {
            // TODO
            RedrawAll?.Invoke();
            Drawer.RefreshArea();
        }

        #region Init handler functions
        private void InitActionHandlers()
        {
            Drawer.LeftMouseDownHandler += HandleLeftMouseDown;
            Drawer.LeftMouseUpHandler += HandleLeftMouseUp;
            Drawer.RightMouseDownHandler += HandleRightMouseDown;
            Drawer.MouseDownMoveHandler += HandleMouseDownMove;
            Drawer.MouseUpMoveHandler += HandleMouseUpMove;
        }

        private void InitBusinessLogicHandlers()
        {
            Drawer.ModeChangedHandler += HandleModeChange;
            Drawer.EdgeInsertPointClickedHandler += HandleEdgePointInsert;
            Drawer.EdgeSetLengthClickedHandler += HandleEdgeSetFixedLength;
            Drawer.SelectedRelationChangedHandler += HandleSelectedRelationChanged;
            Drawer.RelationDeleteHandler += HandleRelationDelete;
            Drawer.EdgeDeleteFixedLengthHandler += HandleEdgeDeleteFixedLength;
        }

        private void HandleEdgeDeleteFixedLength(object sender, EventArgs e)
        {
            Constraints.FixedLengthRepository.RemoveForEdge(SelectedEdge);
            RedrawAll?.Invoke();
            Drawer.RefreshArea();
        }

        private void InitModelChangedHandlers()
        {
            Shapes.OnSolitaryPointAdded += HandleSolitaryPointAdd;
        }
        #endregion

        #region BL handlers
        private void HandleModeChange(object sender, EventArgs e)
        {
            Click = default;
            MovingItem = null;
            SelectedEdge = null;
            MakePerpendicularEdge = null;

            RedrawAll?.Invoke();
            Drawer.RefreshArea();
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
            Constraints.RemoveAllForEdge(SelectedEdge);

            Shapes.GetPolygonByEdge(SelectedEdge).InsertPoint(SelectedEdge, new Point()
            {
                X = (SelectedEdge.U.X + SelectedEdge.V.X) / 2,
                Y = (SelectedEdge.U.Y + SelectedEdge.V.Y) / 2
            });

            RedrawAll?.Invoke();
            Drawer.RefreshArea();
        }

        private void HandleSelectedRelationChanged(object sender, EventArgs e)
        {
            RedrawAll?.Invoke();
            Drawer.RefreshArea();
        }

        private void HandleRelationDelete(object sender, EventArgs e)
        {
            Constraints.PerpendicularRepository.Remove(SelectedRelation as Perpendicular);

            Drawer.UnsetSelectedRelation();
            RefreshRelationsList();
        }
        #endregion

        #region BL helpers

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

            var uv = v - u;
            var newVector = uv * length / uv.Length();
            PointMoveWithConstraints(u, uv - newVector);

            Constraints.FixedLengthRepository.Add(SelectedEdge, length);
            RedrawAll?.Invoke();
            Drawer.RefreshArea();
        }

        private void RefreshRelationsList()
        {
            var relations = Constraints.PerpendicularRepository.GetForEdge(SelectedEdge);
            Drawer.SetRelationsListDataSource(relations.AsEdgeConstraint().ToList());
        }
        #endregion

        #region User action handlers
        private void HandleLeftMouseDown(object sender, MouseEventArgs e)
        {
            Click = e.Location;

            switch (Drawer.Mode)
            {
                case DrawerMode.Draw:
                    {
                        var selectedVertex = Shapes.GetSolitaryPoints().Find(x => x.WasClicked(Click, DrawerClass.PointWidth));

                        if (selectedVertex == default(IPoint))
                        {
                            Drawer.DrawPoint(Click);
                            Shapes.AddSolitaryPoint(Click);
                            Drawer.RefreshArea();
                        }
                        else if (selectedVertex == Shapes.GetSolitaryPoints().First() && Shapes.GetSolitaryPoints().Count > 2)
                        {
                            Shapes.AddPolygon(Shapes.GetSolitaryPoints());
                            Shapes.ClearSolitaryPoints();

                            RedrawAll?.Invoke();
                            Drawer.RefreshArea();
                        }
                        break;
                    }

                case DrawerMode.Delete:
                    {
                        var selectedVertex = Shapes.GetAllPolygonPoints().Find(x => x.WasClicked(Click, DrawerClass.PointWidth));

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
                            Drawer.RefreshArea();
                        }
                        break;
                    }

                case DrawerMode.Modify:
                    {
                        #region Edge selection

                        SelectedEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.EdgeWidth));
                        MovingItem = SelectedEdge;

                        if (MovingItem != default(Edge))
                        {
                            return;
                        }

                        #endregion
                        #region Vertex selection - move

                        MovingItem = Shapes.GetAllPolygonPoints().Find(x => x.WasClicked(Click, DrawerClass.PointWidth));

                        if (MovingItem != default(Point))
                        {
                            return;
                        }

                        #endregion
                        #region Polygon selection - move

                        MovingItem = Shapes.GetAllPolygons().Find(x => x.WasClicked(Click, DrawerClass.MoveIconWidth));

                        #endregion
                        break;
                    }

                case DrawerMode.MakePerpendicular:
                    {
                        var preSelectedEdge = MakePerpendicularEdge;
                        MakePerpendicularEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.EdgeWidth));

                        if (preSelectedEdge != default(Edge))
                        {
                            if (MakePerpendicularEdge != default(Edge) && MakePerpendicularEdge != preSelectedEdge)
                            {
                                var relation = Constraints.PerpendicularRepository.Add(preSelectedEdge, MakePerpendicularEdge);

                                // TODO: some assertion needed (if can make perpendicular) + implement assumption of not making perpendicular edges that are not neighbors
                                QueuedPerpendiculars.Add(relation);
                                SetPerpendicular(relation.Edge, relation.Value);
                                QueuedPerpendiculars.Remove(relation);

                                RedrawAll?.Invoke();
                                Drawer.RefreshArea();
                            }

                            MakePerpendicularEdge = null;
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void HandleLeftMouseUp(object sender, MouseEventArgs e)
        {
            MovingItem = null;
        }

        private void HandleRightMouseDown(object sender, MouseEventArgs e)
        {
            Click = e.Location;

            SelectedEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.EdgeWidth));

            if (SelectedEdge != default(IEdge))
            {
                Drawer.ShowManageEdgeMenu(Click);
            }
        }

        private void HandleMouseDownMove(object sender, MouseEventArgs e)
        {
            if (Drawer.Mode == DrawerMode.Modify)
            {
                var vector = new Vector2
                {
                    X = e.Location.X - Click.X,
                    Y = e.Location.Y - Click.Y
                };
                Click = e.Location;

                MovingItem?.MoveWithConstraints(vector);

                RedrawAll?.Invoke();
                Drawer.RefreshArea();
            }
        }

        private void HandleMouseUpMove(object sender, MouseEventArgs e)
        {
            if (Drawer.Mode == DrawerMode.Draw)
            {
                var solitaryPoints = Shapes.GetSolitaryPoints();
                if (solitaryPoints.Count > 0)
                {
                    RedrawAll?.Invoke();
                    Drawer.DrawLine(solitaryPoints.Last().Center, e.Location);
                    Drawer.RefreshArea();
                }
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

        private void RecolorSelectedRelationEdges()
        {
            if (SelectedRelation is not null)
            {
                var firstEdge = SelectedRelation.Edge;
                var secondEdge = SelectedRelation.Value;

                Drawer.DrawLine(firstEdge.U.Center, firstEdge.V.Center, SpecialColor);
                Drawer.DrawLine(secondEdge.U.Center, secondEdge.V.Center, SpecialColor);
            }
        }
        #endregion

        #region Move with constraints functions
        private void PointMoveWithConstraints(IPoint root, Vector2 rootMove)
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
            QueuedPerpendiculars.Clear();
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

        private bool Algorithm(IPolygon polygon, Queue<(IPoint toMove, Vector2 move)> toBeProcessed)
        {
            var canBeProcessed = polygon.Vertices.ToHashSet();

            var perpendicularsToBeProcessed = new Dictionary<IEdge, IEdge>();
            var perpendicularsFromAnotherPolygon = new List<(IPoint toMove, Vector2 move)>();

            while (toBeProcessed.Any() && canBeProcessed.Count > 0)
            {
                (var u, var move) = toBeProcessed.Dequeue();

                if (canBeProcessed.Remove(u))
                {
                    foreach (var e in polygon.GetNeighborEdges(u))
                    {
                        var v = u.GetNeighbor(e);

                        if (canBeProcessed.Contains(v))
                        {
                            if (perpendicularsToBeProcessed.TryGetValue(e, out IEdge relative)) // it preserves length constraint
                            {
                                var fixedLength = Constraints.FixedLengthRepository.GetForEdge(e).SingleOrDefault()?.Value;
                                var instruction = e.GetMakePerpendicularInstruction(relative, fixedLength);
                                toBeProcessed.Enqueue(instruction);
                                perpendicularsToBeProcessed.Remove(e);
                            }
                            else if (Constraints.FixedLengthRepository.HasConstraint(e))
                            {
                                var vu = u - v;
                                var vu_moved = vu + move;
                                var vMove = vu_moved - Vector2.Normalize(vu_moved) * vu.Length();
                                toBeProcessed.Enqueue((v, vMove));
                            }

                            if (Constraints.PerpendicularRepository.HasConstraint(e))
                            {
                                foreach (var rel in Constraints.PerpendicularRepository.GetForEdge(e))
                                {
                                    if (!QueuedPerpendiculars.Contains(rel))
                                    {
                                        QueuedPerpendiculars.Add(rel);
                                        var relatedEdge = rel.Edge == e ? rel.Value : rel.Edge;
                                        if (polygon.Edges.Contains(relatedEdge))
                                        {
                                            perpendicularsToBeProcessed.Add(relatedEdge, e);
                                        }
                                        else
                                        {
                                            perpendicularsFromAnotherPolygon.Add(relatedEdge.GetMakePerpendicularInstruction(e));
                                        }
                                        }
                                    }
                                // only for case when perpendicular relation is the only relation on 'e'
                                //if (toBeProcessed.Count == 0 || toBeProcessed.Peek().toMove != v)
                                //{
                                    toBeProcessed.Enqueue((v, Vector2.Zero));
                                //}
                            }
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
                var allFixed = true;
                foreach (var e in polygon.Edges)
                {
                    if (!Constraints.FixedLengthRepository.HasConstraint(e))
                    {
                        allFixed = false;
                        break;
                    }
                }

                if (allFixed)
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
            }

            foreach (var (toMove, move) in perpendicularsFromAnotherPolygon)
            {
                PointMoveWithConstraints(toMove, move);
            }

            return true;
        }
        #endregion

        #region Constraint algorithm helpers
        public void SetPerpendicular(IEdge e, IEdge f)
        {
            IPoint u, v, w, z;

            bool intersected = true;
            // check intersection
            if (e.U == f.U)
            {
                v = w = e.U;
                u = e.V;
                z = f.V;
            }
            else if (e.U == f.V)
            {
                v = w = e.U;
                u = e.V;
                z = f.U;
            }
            else if (e.V == f.U)
            {
                v = w = e.V;
                u = e.U;
                z = f.V;
            }
            else if (e.V == f.V)
            {
                v = w = e.V;
                u = e.U;
                z = f.U;
            }
            else
            {
                intersected = false;
                u = e.U;
                v = e.V;

                w = f.U;
                z = f.V;
            }

            var uv = v - u;
            var wz = z - w;

            bool eFixed = false, fFixed = false;
            float eLength, fLength;
            if (Constraints.FixedLengthRepository.HasConstraint(e))
            {
                eFixed = true;
                eLength = Constraints.FixedLengthRepository.GetForEdge(e).Single().Value;
            }
            else
            {
                eLength = uv.Length();
            }

            if (Constraints.FixedLengthRepository.HasConstraint(f))
            {
                fFixed = true;
                fLength = Constraints.FixedLengthRepository.GetForEdge(f).Single().Value;
            }
            else
            {
                fLength = wz.Length();
            }

            bool ePerpend = false, fPerpend = false;
            if (Constraints.PerpendicularRepository.GetForEdge(e).Count() > 1)
            {
                ePerpend = true;
            }

            if (Constraints.PerpendicularRepository.GetForEdge(f).Count() > 1)
            {
                fPerpend = true;
            }

            (IPoint toMove, Vector2 move) instruction;

            if (!intersected || (intersected && eFixed && fFixed) || ePerpend || fPerpend)
            {
                if (ePerpend)
                {
                    instruction = SetPerpendicularByFirstPoint(z, w, fLength, u, v);
                }
                else
                {
                    instruction = SetPerpendicularByFirstPoint(u, v, eLength, w, z);
                }
                instruction.toMove.MoveWithConstraints(instruction.move);
            }
            else 
            {
                if (fFixed)
                {
                    instruction = SetPerpendicularByMiddlePoint(z, v, fLength, u);
                }
                else
                {
                    instruction = SetPerpendicularByMiddlePoint(u, v, eLength, z);
                }
                instruction.toMove.Move(instruction.move);
            }
        }

        private static (IPoint, Vector2) SetPerpendicularByFirstPoint(IPoint u, IPoint v, float fixedLength, IPoint z, IPoint w)
        {
            var uv = v - u;
            var wz = w - z;

            var P = new Vector2(wz.Y, -wz.X);
            P = Vector2.Normalize(P) * fixedLength;

            // check better direction
            var direction = Vector2.Dot(uv, P) / (fixedLength * P.Length());
            if (direction > 0)
            {
                P = Vector2.Negate(P);
            }

            return (u, uv + P);
        }

        private static (IPoint, Vector2) SetPerpendicularByMiddlePoint(IPoint u, IPoint v, float fixedLength, IPoint z)
        {
            var uv = v - u;
            var uz = z - u;

            var x = (float)Math.Pow(fixedLength, 2) / uz.Length();
            var T = uv - uz * Vector2.Dot(uv, uz) / Vector2.Dot(-uz, -uz);
            var H = Vector2.Normalize(T) * (float)Math.Sqrt(x * uz.Length() - Math.Pow(x, 2));
            var X = Vector2.Normalize(uz) * x;
            return (v, -uv + X + H);
        }
        #endregion
    }
}