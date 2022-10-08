using Project_1.Models.Repositories;
using Project_1.Models.Shapes;
using Project_1.Views;
using System.Linq;
using System.Collections.Specialized;
using System.Windows.Forms;

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

        public Canvas(IDrawer drawer, IShapeRepository shapes, IRelationRepository relations)
        {
            _drawer = drawer;
            _shapes = shapes;
            _relations = relations;

            InitModelChangedHandlers();
            InitActionHandlers();
        }

        public void InitActionHandlers()
        {
            Drawer.LeftMouseDownHandler += HandleLeftMouseDownEvent;
            Drawer.RightMouseDownHandler += HandleRightMouseDownEvent;
            Drawer.MouseDownMoveHandler += HandleMouseDownMoveEvent;
        }

        public void InitModelChangedHandlers()
        {
            Shapes.OnSolitaryPointAdded += HandleSolitaryPointAdded;
        }

        public void HandleLeftMouseDownEvent(object sender, MouseEventArgs e)
        {
            var clickedPoint = e.Location;
            var clickedInsidePoint = Shapes.GetSolitaryPoints().Find(x => x.IsInside(clickedPoint));

            if (Drawer.IsInDrawingMode)
            {
                if (clickedInsidePoint == default(Point))
                {
                    Drawer.DrawPoint(clickedPoint);
                    Shapes.AddSolitaryPoint(clickedPoint);
                    Drawer.RefreshArea();
                }
                else if (clickedInsidePoint == Shapes.GetSolitaryPoints().First() && Shapes.GetSolitaryPoints().Count > 2)
                {
                    Shapes.AddPolygon(new Polygon
                    {
                        Vertices = Shapes.GetSolitaryPoints()
                    });

                    Shapes.ClearSolitaryPoints();
                    Drawer.ClearArea();
                    Drawer.DrawPolygons(Shapes.GetAllPolygons());
                    Drawer.RefreshArea();
                }
            }
            else if (Drawer.IsInDeleteMode)
            {

            }
        }

        public void HandleRightMouseDownEvent(object sender, MouseEventArgs e)
        {

        }

        public void HandleMouseDownMoveEvent(object sender, MouseEventArgs e)
        {

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
    }
}
