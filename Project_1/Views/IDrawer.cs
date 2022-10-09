using Project_1.Helpers.UI;
using Project_1.Models.Shapes;
using System;
using System.Collections.Generic;

namespace Project_1.Views
{
    public interface IDrawer : IUserActionHandler
    {
        bool IsInDrawingMode { get; }
        bool IsInDeleteMode { get; }
        bool IsInMoveMode { get; }

        event EventHandler ModeChangedHandler;
        event EventHandler EdgeInsertPointClickedHandler;
        event EventHandler EdgeSetFixedLengthClickedHandler;

        void DrawLine(Point p1, Point p2);
        void DrawPoint(Point p);
        void DrawPolygon(Polygon polygon);
        void DrawPolygons(IEnumerable<Polygon> polygons);
        void ClearArea();
        void RefreshArea();
        void ShowManageEdgeMenu(System.Drawing.Point point);
    }
}
