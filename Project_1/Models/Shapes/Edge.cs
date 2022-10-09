namespace Project_1.Models.Shapes
{
    public class Edge : Shape
    {
        public Point U { get; set; }
        public Point V { get; set; }

        public Edge(Point u, Point v) : base()
        {
            U = u;
            V = v;
        }

        public int PolygonId => U.PolygonId;

        public override void Move(System.Drawing.Point vector)
        {
            U.Move(vector);
            V.Move(vector);
        }
    }
}
