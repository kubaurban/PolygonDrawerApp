using System.Windows.Forms;

namespace Project_1.Helpers.UI
{
    public interface IUserActionHandler
    {
        // Mouse events
        event MouseEventHandler LeftMouseDownHandler;
        event MouseEventHandler RightMouseDownHandler;
        event MouseEventHandler MouseDownMoveHandler;
    }
}