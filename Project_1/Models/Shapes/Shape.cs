using System.Numerics;

namespace Project_1.Models.Shapes
{
    public abstract class Shape
    {
        public int Id { get; private set; }

        protected Shape() { }

        protected Shape(int id) => Id = id;

        public abstract void Move(Vector2 vector);
    }
}
