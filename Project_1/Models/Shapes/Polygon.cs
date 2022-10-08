using System.Collections.Generic;

namespace Project_1.Models.Shapes
{
    public class Polygon : Shape
    {
        public List<Point> Vertices { get; set; }

        public Polygon(List<Point> vertices)
        {
            Vertices = vertices;
            Vertices.ForEach(x => x.PolygonId = Id);
        }
    }
}
