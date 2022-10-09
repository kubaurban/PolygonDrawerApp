namespace Project_1.Models.Shapes
{
    public class Point : Shape
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int PolygonId { get; set; }

        public Point(int id) : base(id) { }

        private Point(float x, float y) : base()
        {
            X = x;
            Y = y;
        }

        public override void Move(System.Drawing.Point vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        public static implicit operator System.Drawing.PointF(Point p) => new(p.X, p.Y);
        public static implicit operator Point(System.Drawing.Point p) => new(p.X, p.Y);
    }
}
