using Hardcodet.Wpf.TaskbarNotification;
using Jarvis_Windows.Sources.Utils.Services;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.MVVM.Views.InjectionAction
{
    /// <summary>
    /// Interaction logic for JarvisActionView.xaml
    /// </summary>
    public partial class InjectionActionView : UserControl
    {
        private readonly Size _expandSize = new Size(50, 30);
        private readonly Size _collapseSize = new Size(30, 30);

        public Size ExpandSize => _expandSize;
        public Size CollapseSize => _collapseSize;

        public InjectionActionView()
        {
            InitializeComponent();

            parrentBorder.Background = new SolidColorBrush(Colors.White);
            parrentBorder.Width = _collapseSize.Width;
            parrentBorder.Height = _collapseSize.Height;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if(!PopupDictionaryService.HasPinnedJarvisButton)
            {
                parrentBorder.Width = ExpandSize.Width;
                parrentBorder.Height = ExpandSize.Height;
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            parrentBorder.Width = CollapseSize.Width;
            parrentBorder.Height = CollapseSize.Height;
        }
        Point anchorPoint;
        Point currentPoint;
        bool isInDrag = false;
        private TranslateTransform transform = new TranslateTransform();
        /*private void root_MouseMove(object sender, MouseEventArgs e)
        {
            if (isInDrag)
            {
                var element = sender as FrameworkElement;
                currentPoint = e.GetPosition(null);

                transform.X += currentPoint.X - anchorPoint.X;
                transform.Y += (currentPoint.Y - anchorPoint.Y);
                this.RenderTransform = transform;
                anchorPoint = currentPoint;
            }
        }*/
        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("root_MouseLeftButtonDown");
            var element = sender as FrameworkElement;
            anchorPoint = e.GetPosition(null);
            element.CaptureMouse();
            isInDrag = true;
            e.Handled = true;
        }

        private void root_MouseMove(object sender, MouseEventArgs e)
        {
            if (isInDrag)
            {
                Debug.WriteLine("root_MouseMove");
                var element = sender as FrameworkElement;
                currentPoint = e.GetPosition(null);

                var transform = new TranslateTransform
                {
                    X = (currentPoint.X - anchorPoint.X),
                    Y = (currentPoint.Y - anchorPoint.Y)
                };
                this.RenderTransform = transform;
                anchorPoint = currentPoint;
            }
        }

        private void root_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isInDrag)
            {
                Debug.WriteLine("root_MouseLeftButtonUp");
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                isInDrag = false;
                e.Handled = true;
            }
        }
    }
}
