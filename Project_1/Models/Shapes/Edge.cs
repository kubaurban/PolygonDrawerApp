namespace Project_1.Models.Shapes
{
    public class Edge : Shape
    {
        public Point U { get; set; }
        public Point V { get; set; }

        public override void Move(System.Drawing.Point vector)
        {
            U.Move(vector);
            V.Move(vector);
        }
    }
}
