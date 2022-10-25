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
        private IEnumerable<IEdge> HighlightedEdges { get; set; }
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
            RedrawAll += DisplayPerpendicularIcons;
            RedrawAll += RecolorSelectedRelationEdges;
            RedrawAll += RecolorHighlightedEdges;
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
            #region Polygon 'U'
            var poly1 = Shapes.AddPolygon(new List<IPoint>()
            {
                new Point(104.917351f, 72.2116241f),
                new Point(240, 40),
                new Point(372.486237f, 118.601837f),
                new Point(593.7294f, 37.9027557f),
                new Point(464.932678f, 239.198273f),
                new Point(449.759552f, 351.778442f),
                new Point(74, 301),
            });

            Constraints.PerpendicularRepository.Add(poly1.Edges[4], poly1.Edges[5]);
            Constraints.PerpendicularRepository.Add(poly1.Edges[5], poly1.Edges[6]);

            Constraints.FixedLengthRepository.Add(poly1.Edges[0], poly1.Edges[0].Length);
            Constraints.FixedLengthRepository.Add(poly1.Edges[3], poly1.Edges[3].Length);
            #endregion

            #region Polygon perpendicular
            var poly2 = Shapes.AddPolygon(new List<IPoint>()
            {
                new Point(804.917358f, 72.2116241f),
                new Point(1040, 40),
                new Point(972.486237f, 118.601837f),
                new Point(1069.62183f, 383.937073f),
                new Point(940, 301),
            });

            Constraints.PerpendicularRepository.Add(poly1.Edges[3], poly2.Edges[3]);

            Constraints.FixedLengthRepository.Add(poly2.Edges[2], poly2.Edges[2].Length);
            #endregion

            #region Polygon multiple fixed edges + perpendicular
            var poly3 = Shapes.AddPolygon(new List<IPoint>()
            {
                new Point(852.4634f, 346.9718f),
                new Point(758.567139f, 307.212219f),
                new Point(660.7959f, 339.234253f),
                new Point(587.3084f, 401.331055f),
                new Point(557.567f, 478.137329f),
                new Point(726.825f, 543.678467f),
                new Point(821.7804f, 492.0345f),
            });

            Constraints.FixedLengthRepository.Add(poly3.Edges[0], poly3.Edges[0].Length);
            Constraints.FixedLengthRepository.Add(poly3.Edges[4], poly3.Edges[4].Length);
            Constraints.FixedLengthRepository.Add(poly3.Edges[5], poly3.Edges[5].Length);
            Constraints.FixedLengthRepository.Add(poly3.Edges[6], poly3.Edges[6].Length);

            Constraints.PerpendicularRepository.Add(poly3.Edges[3], poly3.Edges[4]);
            #endregion

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
            Drawer.MouseMiddleHandler += HandleMouseMiddleDown;
        }

        private void HandleMouseMiddleDown(object sender, MouseEventArgs e)
        {
            var edge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(e.Location, DrawerClass.EdgeWidth));
            if (edge != null)
            {
                MakeBezier(edge);
                RedrawAll?.Invoke();
                Drawer.RefreshArea();
            }
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

        private void SetSelectedEdgeLength(float length)
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
                        #region Polygon selection - move

                        MovingItem = Shapes.GetAllPolygons().Find(x => x.WasClicked(Click, DrawerClass.MoveIconWidth));

                        if (MovingItem != default(Polygon))
                        {
                            return;
                        }

                        #endregion
                        #region Edge selection

                        SelectedEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.EdgeWidth));
                        MovingItem = SelectedEdge;

                        if (MovingItem != default(Edge))
                        {
                            HighlightedEdges = Constraints.PerpendicularRepository.GetForEdge(SelectedEdge).SelectMany(x => new HashSet<IEdge>() { x.Edge, x.Value });

                            return;
                        }

                        #endregion
                        #region Vertex selection - move

                        var selectedVertex = Shapes.GetAllPolygonPoints().Find(x => x.WasClicked(Click, DrawerClass.PointWidth));
                        MovingItem = selectedVertex;

                        if (MovingItem != default(Point))
                        {
                            var relatedEdges = Shapes.GetPolygonByPoint(selectedVertex).GetNeighborEdges(selectedVertex);

                            HighlightedEdges = relatedEdges.SelectMany(x =>
                                Constraints.PerpendicularRepository.GetForEdge(x)
                                    .SelectMany(y => new HashSet<IEdge>() { y.Edge, y.Value }));

                            return;
                        }

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
            HighlightedEdges = null;
            RedrawAll?.Invoke();
            Drawer.RefreshArea();
        }

        private void HandleRightMouseDown(object sender, MouseEventArgs e)
        {
            if (Drawer.Mode == DrawerMode.Modify)
            {
                Click = e.Location;

                SelectedEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.EdgeWidth));

                if (SelectedEdge != default(IEdge))
                {
                    Drawer.ShowManageEdgeMenu(Click, Constraints.FixedLengthRepository.HasConstraint(SelectedEdge));
                }
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
                var length = Constraints.FixedLengthRepository.GetForEdge(e).SingleOrDefault()?.Value;
                if (length.HasValue)
                {
                    WriteEdgeLength(e, (int)length.Value);
                }
            }
        }

        private void WriteEdgeLength(IEdge edge, int length)
        {
            Drawer.Write(edge.Center, length.ToString());
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

        private void RecolorHighlightedEdges()
        {
            if (HighlightedEdges is not null)
            {
                foreach (var e in HighlightedEdges)
                {
                    Drawer.DrawLine(e.U.Center, e.V.Center, SpecialColor);
                }
            }
        }

        private void DisplayPerpendicularIcons()
        {
            // code below written very fast, just before labs
            var processedRelations = new HashSet<Perpendicular>();
            foreach (var edge in Shapes.GetAllPolygonEdges())
            {
                string label = string.Empty;
                foreach (var rel in Constraints.PerpendicularRepository.GetForEdge(edge))
                {
                    processedRelations.Add(rel);

                    var letter = (char)('A' + rel.Id);
                    label += letter.ToString() + ' ';
                }

                var vec = new Vector2(-20, -10);
                var coord = new System.Drawing.PointF(edge.Center.X + vec.X, edge.Center.Y + vec.Y);
                Drawer.Write(coord, label, SpecialColor);
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

            var relations = Constraints.PerpendicularRepository.GetForEdge(root).Where(x => polygon.Edges.Contains(x.Edge) && polygon.Edges.Contains(x.Value));

            if (!relations.Any())
            {
                toBeProcessed.Enqueue((root.U, rootMove));
                toBeProcessed.Enqueue((root.V, rootMove));
            }
            else
            {
                #region preprocessing if neighboring edges are related to moved edge
                var uNeigh = polygon.GetNeighborEdges(root.U).Single(x => x != root); // e
                var vNeigh = polygon.GetNeighborEdges(root.V).Single(x => x != root); // f

                // fixed length check
                var eLength = Constraints.FixedLengthRepository.GetForEdge(uNeigh).SingleOrDefault()?.Value;
                var fLength = Constraints.FixedLengthRepository.GetForEdge(vNeigh).SingleOrDefault()?.Value;

                // perpendicualr with root check
                var uNeighPerpend = relations.Any(x => x.Edge == uNeigh || x.Value == uNeigh);
                var vNeighPerpend = relations.Any(x => x.Edge == vNeigh || x.Value == vNeigh);

                root.U.Move(rootMove);
                root.V.Move(rootMove);

                var uv = root.V - root.U;
                var perpendNotFixedMove = -uv * Vector2.Dot(rootMove, -uv) / Vector2.Dot(uv, uv);
                if (uNeighPerpend)
                {
                    toBeProcessed.Enqueue((root.U, Vector2.Zero));

                    var toMove = root.U.GetNeighbor(uNeigh);
                    if (eLength.HasValue)
                    {
                        var instr = SetPerpendicularByFirstPoint(toMove, root.U, eLength.Value, root.U, root.V);
                        toBeProcessed.Enqueue((instr.toMove, instr.move));
                    }
                    else
                    {
                        toBeProcessed.Enqueue((toMove, perpendNotFixedMove));
                    }
                }

                if (vNeighPerpend)
                {
                    toBeProcessed.Enqueue((root.V, Vector2.Zero));

                    var toMove = root.V.GetNeighbor(vNeigh);
                    if (fLength.HasValue)
                    {
                        var instr = SetPerpendicularByFirstPoint(toMove, root.V, fLength.Value, root.V, root.U);
                        toBeProcessed.Enqueue((instr.toMove, instr.move));
                    }
                    else
                    {
                        toBeProcessed.Enqueue((toMove, perpendNotFixedMove));
                    }
                }

                if (!uNeighPerpend && eLength.HasValue)
                {
                    root.U.Move(-rootMove);
                    toBeProcessed.Enqueue((root.U, rootMove));
                }

                if (!vNeighPerpend && fLength.HasValue)
                {
                    root.V.Move(-rootMove);
                    toBeProcessed.Enqueue((root.V, rootMove));
                }

                foreach (var rel in relations)
                {
                    QueuedPerpendiculars.Add(rel);
                }
                #endregion
            }

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
                                            if (!perpendicularsToBeProcessed.ContainsKey(relatedEdge))
                                            {
                                                perpendicularsToBeProcessed.Add(relatedEdge, e);
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            perpendicularsFromAnotherPolygon.Add(SetPerpendicularByFirstPoint(relatedEdge.U, relatedEdge.V, relatedEdge.Length, e.U, e.V));
                                        }
                                    }
                                }
                                toBeProcessed.Enqueue((v, Vector2.Zero));
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

        private static (IPoint toMove, Vector2 move) SetPerpendicularByFirstPoint(IPoint u, IPoint v, float fixedLength, IPoint z, IPoint w)
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

        private void MakeBezier(IEdge e)
        {
            e.Bezier = true;
            Constraints.RemoveAllForEdge(e);
        }
    }
}