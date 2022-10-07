using Project_1.Models.Shapes;

﻿namespace Project_1.Views
{
    public interface IDrawer
    {
        public void DrawLine(Point p1, Point p2);
        public void DrawPoint(Point p);
        public void DrawPolygon(Polygon polygon);
        public void ClearArea();
        public void RefreshArea();
    }
}
