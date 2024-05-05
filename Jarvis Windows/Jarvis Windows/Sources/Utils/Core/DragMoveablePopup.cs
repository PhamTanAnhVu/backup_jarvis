using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Jarvis_Windows.Sources.Utils.Core
{
    public class DragMoveablePopup : Popup
    {
        public DragMoveablePopup()
        {
            this.MouseLeftButtonDown += DragMoveablePopup_MouseLeftButtonDown;
            this.MouseMove += DragMoveablePopup_MouseMove;
            this.MouseLeftButtonUp += DragMoveablePopup_MouseLeftButtonUp;
        }

        private void DragMoveablePopup_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
        }

        private void DragMoveablePopup_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                System.Windows.Point point = e.GetPosition(this);
                this.HorizontalOffset = point.X;
                this.VerticalOffset = point.Y;
                Debug.WriteLine("Popup new pos: %d - %d", point.X, point.Y);
            }
        }

        private void DragMoveablePopup_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.CaptureMouse();
        }
    }
}
