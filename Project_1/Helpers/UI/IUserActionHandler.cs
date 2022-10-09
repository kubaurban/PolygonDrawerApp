using System.Windows.Forms;

namespace Project_1.Helpers.UI
{
    public interface IUserActionHandler
    {
        // Mouse events
        event MouseEventHandler LeftMouseDownHandler;
        event MouseEventHandler LeftMouseUpHandler;
        event MouseEventHandler RightMouseDownHandler;
        event MouseEventHandler MouseDownMoveHandler;
    }
}