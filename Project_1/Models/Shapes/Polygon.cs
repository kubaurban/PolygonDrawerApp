using System.Collections.Generic;

namespace Project_1.Models.Shapes
{
    public class Polygon : Shape
    {
        private List<Point> _vertices;

        public List<Point> Vertices { 
            get => _vertices;
            set
            {
                _vertices = value;
                _vertices.ForEach(x => x.PolygonId = Id);
            }
        }

        public Polygon(int id) : base(id) { }

        public void RemoveVertex(Point p)
        {
            Vertices.Remove(p);
        }
    }
}
