using Project_1.Helpers.UI;
using Project_1.Models.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Project_1.Views
{
    public interface IDrawer : IUserActionHandler
    {
        DrawerMode Mode { get; set; }

        event EventHandler ModeChangedHandler;
        event EventHandler EdgeInsertPointClickedHandler;
        event EventHandler EdgeSaveLengthClickedHandler;

        void DrawLine(PointF p1, PointF p2);
        void DrawPoint(PointF p);
        void DrawPolygon(Polygon polygon);
        void DrawPolygons(IEnumerable<Polygon> polygons);
        void ClearArea();
        void RefreshArea();
        void ShowManageEdgeMenu(PointF point);
    }
}
