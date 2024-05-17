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
    }
}
