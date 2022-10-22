namespace Project_1.Models.Shapes.Abstract
{
    public interface IEdge : IShape
    {
        IPoint U { get; set; }
        IPoint V { get; set; }

        int Length { get; }
    }
}