namespace Project_1.Models.Shapes
{
    public class Point : Shape
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator System.Drawing.PointF(Point p) => new(p.X, p.Y);
        public static implicit operator Point(System.Drawing.PointF p) => new(p.X, p.Y);
    }
}
