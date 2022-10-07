using Project_1.Models.Shapes;

﻿namespace Project_1.Views
{
    public interface IDrawer
    {
        void DrawLine(Point p1, Point p2);
        void DrawPoint(Point p);
        void DrawPolygon(Polygon polygon);
        void ClearArea();
        void RefreshArea();
    }
}
