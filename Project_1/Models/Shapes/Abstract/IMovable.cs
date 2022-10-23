using System.Numerics;

namespace Project_1.Models.Shapes.Abstract
{
    public interface IMovable
    {
        public void Move(Vector2 vector);

        public void MoveWithConstraints(Vector2 vector);
    }
}
