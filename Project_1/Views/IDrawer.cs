using Project_1.Helpers.UI;
using Project_1.Models.Shapes.Abstract;
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
        event EventHandler EdgeSetLengthClickedHandler;

        void DrawLine(PointF p1, PointF p2);
        void DrawPoint(PointF p);
        void DrawPolygon(IPolygon polygon);
        void DrawPolygons(IEnumerable<IPolygon> polygons);
        void ClearArea();
        void RefreshArea();
        void ShowManageEdgeMenu(PointF point);
    }
}
