using System.Numerics;

namespace Project_1.Models.Shapes
{
    public abstract class Shape
    {
        protected Shape() { }

        public abstract void Move(Vector2 vector);
    }
}
