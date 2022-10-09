using System.Drawing;

namespace Project_1.Helpers.UI
{
    public static class PointFExtension
    {
        public static Point ToPoint(this PointF point) => new((int)point.X, (int)point.Y);
    }
}
