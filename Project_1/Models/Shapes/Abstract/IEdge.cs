using System.Numerics;

namespace Project_1.Models.Shapes.Abstract
{
    public delegate void EdgeMoveWithConstraints(IEdge shape, Vector2 vector);

    public interface IEdge : IShape
    {
        IPoint U { get; set; }
        IPoint V { get; set; }

        int Length { get; }
        void MakePerpendicularWithConstraints(IEdge edge);
        (IPoint toMove, Vector2 move) GetMakePerpendicularInstruction(IEdge edge, int? fixedLength = null);
    }
}