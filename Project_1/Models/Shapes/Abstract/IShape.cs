using System.Drawing;

namespace Project_1.Models.Shapes.Abstract
{
    public interface IShape : IMovable
    {
        PointF Center { get; }

        bool WasClicked(PointF click, int clickRadius);
    }
}
