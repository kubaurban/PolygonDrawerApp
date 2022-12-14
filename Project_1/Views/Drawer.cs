using Project_1.Helpers.UI;
using Project_1.Models.Constraints.Abstract;
using Project_1.Models.Shapes.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Project_1.Views
{
    delegate void LineDrawer(PointF p1, PointF p2, Color? color);

    public partial class Drawer : Form, IDrawer
    {
        #region Private fields

        private static readonly int _pointWidth = 10;
        private static readonly int _edgeClickWidth = 10;
        private static readonly int _moveIconWidth = 20;

        #endregion

        #region User action handlers

        public event MouseEventHandler LeftMouseDownHandler;
        public event MouseEventHandler LeftMouseUpHandler;
        public event MouseEventHandler RightMouseDownHandler;
        public event MouseEventHandler MouseDownMoveHandler;
        public event MouseEventHandler MouseUpMoveHandler;

        #endregion

        #region BL handlers

        public event EventHandler ModeChangedHandler;
        public event EventHandler EdgeInsertPointClickedHandler;
        public event EventHandler EdgeSetLengthClickedHandler;
        public event EventHandler SelectedRelationChangedHandler;
        public event EventHandler RelationDeleteHandler;
        public event EventHandler EdgeDeleteFixedLengthHandler;

        #endregion

        public static int PointWidth => _pointWidth;
        public static int EdgeWidth => _edgeClickWidth;
        public static int MoveIconWidth => _moveIconWidth;
        public static string MoveIconFilePath => "../../../../Resources/move.ico";

        private Bitmap DrawArea { get; }
        private Graphics Graphics => Graphics.FromImage(DrawArea);
        private Color DefaultColor { get; }
        private Brush DefaultBrush { get; }
        private Font DefaultWrittingFont { get; }
        private LineDrawer LineDrawer { get; set; }

        public DrawerMode Mode { get; private set; }
        private bool IsLeftMouseDown { get; set; }

        public Drawer()
        {
            InitializeComponent();

            #region Init UI components
            InitManageEdgeMenuItems();
            InitManageEdgeRelationMenuItems();
            #endregion

            DrawArea = new Bitmap(PictureBox.Width, PictureBox.Height);
            PictureBox.Image = DrawArea;

            #region Default settings
            DefaultColor = Color.Black;
            DefaultBrush = new SolidBrush(DefaultColor);
            DefaultWrittingFont = new Font("Arial", 8);
            #endregion

            InitDefaultState();

            #region Mode checked handlers

            DrawingMode.CheckedChanged += OnDrawingModeChecked;
            DeleteMode.CheckedChanged += OnDeleteModeChecked;
            ModifyMode.CheckedChanged += OnModifyModeChecked;
            MakePerpendicularMode.CheckedChanged += OnMakePerpendicularModeChecked;

            #endregion

            ClearArea();
            RefreshArea();
        }

        public Form GetForm() => this;

        private void InitDefaultState()
        {
            Mode = DrawerMode.Draw;
            IsLeftMouseDown = false;
            LineDrawer = DrawLineLibrary;
        }

        #region Manage edge menu
        private void InitManageEdgeMenuItems()
        {
            var addMiddle = new ToolStripMenuItem("Insert point", null, new EventHandler(OnEdgeInsertPoint));
            var setLength = new ToolStripMenuItem("Set fixed length", null, new EventHandler(OnEdgeSetFixedLength));
            var deleteFixedLength = new ToolStripMenuItem("Delete fixed length", null, new EventHandler(OnEdgeDeleteFixedLength))
            {
                Enabled = false
            };
            ManageEdgeMenu.Items.AddRange(new ToolStripItem[] { addMiddle, setLength, deleteFixedLength });
        }

        public void ShowManageEdgeMenu(PointF point, bool isFixed)
        {
            ManageEdgeMenu.Items[2].Enabled = isFixed;
            ManageEdgeMenu.Show(PictureBox, new Point((int)point.X, (int)point.Y));
        }

        private void OnEdgeInsertPoint(object sender, EventArgs e)
        {
            EdgeInsertPointClickedHandler?.Invoke(sender, e);
        }

        private void OnEdgeSetFixedLength(object sender, EventArgs e)
        {
            EdgeSetLengthClickedHandler?.Invoke(sender, e);
        }

        private void OnEdgeDeleteFixedLength(object sender, EventArgs e)
        {
            EdgeDeleteFixedLengthHandler?.Invoke(sender, e);
        }
        #endregion

        #region Manage relation menu
        private void InitManageEdgeRelationMenuItems()
        {
            var deleteRelation = new ToolStripMenuItem("Delete", null, new EventHandler(OnEdgeRelationDelete));
            ManageEdgeRelationMenu.Items.Add(deleteRelation);
        }

        public void ShowManageEdgeRelationMenu(PointF point)
        {
            ManageEdgeRelationMenu.Show(RelationsList, new Point((int)point.X, (int)point.Y));
        }

        private void OnEdgeRelationDelete(object sender, EventArgs e)
        {
            RelationDeleteHandler?.Invoke(sender, e);
        }
        #endregion

        #region Relations box
        public void EnableRelationsBoxVisibility() => RelationsBox.Visible = true;

        public void DisableRelationsBoxVisibility() => RelationsBox.Visible = false;

        public void SetRelationsListDataSource(IList<IEdgeConstraint<IEdge>> relations)
        {
            RelationsList.DataSource = relations;
        }

        public IEdgeConstraint<IEdge> GetSelectedRelation() => RelationsList.SelectedItem as IEdgeConstraint<IEdge>;

        public void UnsetSelectedRelation() => RelationsList.ClearSelected();

        private void OnSelectedRelationChanged(object sender, EventArgs e)
        {
            SelectedRelationChangedHandler?.Invoke(sender, e);
        }

        private void OnRelationsListMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var clickedIndex = RelationsList.IndexFromPoint(e.Location);
                if (clickedIndex != ListBox.NoMatches)
                {
                    RelationsList.SelectedItem = RelationsList.Items[clickedIndex];
                    ShowManageEdgeRelationMenu(e.Location);
                }
            }
        }
        #endregion

        #region Bresenham logic
        private void DisableBresenhamAlgorithm() => IsBresenham.Enabled = false;

        private void EnableBresenhamAlgorithm() => IsBresenham.Enabled = true;

        private void DeleteModeChecked(object sender, EventArgs e)
        {
            DisableBresenhamAlgorithm();
        }

        private void DrawingModeChecked(object sender, EventArgs e)
        {
            EnableBresenhamAlgorithm();
        }

        private void ModifyModeChecked(object sender, EventArgs e)
        {
            DisableBresenhamAlgorithm();
        }

        private void MakePerpendicularModeChecked(object sender, EventArgs e)
        {
            DisableBresenhamAlgorithm();
        }

        private void IsBresenhamCheckedChanged(object sender, EventArgs e)
        {
            if (IsBresenham.Checked)
            {
                LineDrawer = DrawLineBresenham;
            }
            else
            {
                LineDrawer = DrawLineLibrary;
            }
        }
        #endregion

        #region Drawing shapes
        public void DrawLine(PointF p1, PointF p2, Color? color = null)
        {
            LineDrawer?.Invoke(p1, p2, color ?? DefaultColor);
        }

        private void DrawLineLibrary(PointF p1, PointF p2, Color? color = null)
        {
            using var g = Graphics;
            g.DrawLine(new(color ??= DefaultColor, color == DefaultColor ? 1 : 2), p1, p2);
        }

        private void DrawLineBresenham(PointF p1, PointF p2, Color? color = null)
        {
            var drawColor = color is null ? DefaultColor : color.Value;

            var dx = Math.Abs((int)p2.X - (int)p1.X);
            var dy = Math.Abs((int)p2.Y - (int)p1.Y);
            var d_horizontal = (p1.X < p2.X) ? 1 : p1.X == p2.X ? 0 : -1;
            var d_vertical = (p1.Y < p2.Y) ? 1 : p1.Y == p2.Y ? 0 : -1;

            var x = (int)p1.X;
            var y = (int)p1.Y;
            DrawArea.SetPixel(x, y, drawColor);

            if (dx > dy)
            {
                int d = 2 * dy - dx;
                while (x != p2.X)
                {
                    if (d < 0) // choose E
                    {
                        d += 2 * dy;
                        x += d_horizontal;
                    }
                    else // choose NE
                    {
                        d += (dy - dx) * 2;
                        x += d_horizontal;
                        y += d_vertical;
                    }
                    DrawArea.SetPixel(x, y, drawColor);
                }
            }
            else
            {
                int d = 2 * dx - dy;
                while (y != p2.Y)
                {
                    if (d < 0) // choose E
                    {
                        d += 2 * dx;
                        y += d_vertical;
                    }
                    else // choose NE
                    {
                        d += (dx - dy) * 2;
                        x += d_horizontal;
                        y += d_vertical;
                    }
                    DrawArea.SetPixel(x, y, drawColor);
                }
            }
        }

        public void DrawPoint(PointF p)
        {
            using var g = Graphics;
            g.FillRectangle(DefaultBrush, p.X - PointWidth / 2, p.Y - PointWidth / 2, PointWidth, PointWidth);
        }

        public void DrawPolygon(IPolygon polygon)
        {
            using var g = Graphics;

            var first = polygon.Vertices.First();
            DrawPoint(first.Center);
            var prev = first;
            foreach (var v in polygon.Vertices.Skip(1))
            {
                DrawPoint(v.Center);
                DrawLineLibrary(prev.Center, v.Center);
                prev = v;
            }
            DrawLineLibrary(prev.Center, first.Center);

            if (Mode == DrawerMode.Modify)
            {
                // draw gravity center point
                DrawGrabIcon(polygon.Center);
            }
        }

        public void DrawPolygons(IEnumerable<IPolygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                DrawPolygon(polygon);
            }
        }

        public void DrawGrabIcon(PointF point)
        {
            using var g = Graphics;
            g.DrawIcon(new(MoveIconFilePath), new((int)point.X - MoveIconWidth / 2, (int)point.Y - MoveIconWidth / 2, MoveIconWidth, MoveIconWidth));
        }

        public void Write(PointF center, string text)
        {
            var point = center;

            using var g = Graphics;
            g.DrawString(text, DefaultWrittingFont, DefaultBrush, point);
        }

        public void ClearArea()
        {
            using var g = Graphics;
            g.Clear(Color.White);
        }

        public void RefreshArea()
        {
            PictureBox.Refresh();
        }
        #endregion

        #region User action event handling functions
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    IsLeftMouseDown = true;
                    LeftMouseDownHandler?.Invoke(sender, e);
                    break;
                case MouseButtons.Right:
                    RightMouseDownHandler?.Invoke(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsLeftMouseDown)
            {
                MouseDownMoveHandler?.Invoke(sender, e);
            }
            else
            {
                MouseUpMoveHandler?.Invoke(sender, e);
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsLeftMouseDown = false;
                LeftMouseUpHandler?.Invoke(sender, e);
            }
        }

        private void OnDrawingModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.Draw;
            OnModeChanged(sender, e);
        }

        private void OnDeleteModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.Delete;
            OnModeChanged(sender, e);
        }

        private void OnModifyModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.Modify;
            OnModeChanged(sender, e);
        }

        private void OnMakePerpendicularModeChecked(object sender, EventArgs e)
        {
            Mode = DrawerMode.MakePerpendicular;
            OnModeChanged(sender, e);
        }

        private void OnModeChanged(object sender, EventArgs e)
        {
            ModeChangedHandler?.Invoke(sender, e);
        }
        #endregion
    }
}