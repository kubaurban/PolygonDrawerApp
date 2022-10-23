﻿using Project_1.Helpers.BL;
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

        public Canvas(IDrawer drawer, IShapeRepository shapes, IConstraintRepositories constraintRepositories)
        {
            Drawer = drawer;
            Shapes = shapes;
            Constraints = constraintRepositories;

            _selectedEdge = null;

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
            Constraints.FixedLengthRepository.RemoveForEdge(SelectedEdge);

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

                        SelectedEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.PointWidth));
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
                        MakePerpendicularEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.PointWidth));

                        if (preSelectedEdge != default(Edge))
                        {
                            if (MakePerpendicularEdge != default(Edge) && MakePerpendicularEdge != preSelectedEdge)
                            {
                                var relation = Constraints.PerpendicularRepository.Add(preSelectedEdge, MakePerpendicularEdge);

                                relation.Edge.MakePerpendicularWithConstraints(relation.Value);

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

            SelectedEdge = Shapes.GetAllPolygonEdges().Find(x => x.WasClicked(Click, DrawerClass.PointWidth));

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

        private bool Algorithm(IPolygon polygon, Queue<(IPoint, Vector2)> toBeProcessed)
        {
            var canBeProcessed = polygon.Vertices.ToHashSet();
            
            var queuedPerpendiculars = new HashSet<Perpendicular>();
            var perpendicularsToBeProcessed = new Dictionary<IEdge, IEdge>();

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
                                var instruction = e.GetMakePerpendicularInstruction(relative);
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
                                    // for now we assume max one loop (one neighboring perpendicular edge)
                                    if (!queuedPerpendiculars.Contains(rel))
                                    {
                                        perpendicularsToBeProcessed.Add(rel.Edge == e ? rel.Value : rel.Edge, e);
                                    }
                                    queuedPerpendiculars.Add(rel);
                                    toBeProcessed.Enqueue((v, Vector2.Zero));
                                }
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
        #endregion
    }
}
