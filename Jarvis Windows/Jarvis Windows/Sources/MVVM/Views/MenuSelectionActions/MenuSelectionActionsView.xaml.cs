using Hardcodet.Wpf.TaskbarNotification;
using Jarvis_Windows.Sources.Utils.Services;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions
{
    /// <summary>
    /// Interaction logic for MenuSelectionActionsView.xaml
    /// </summary>
    public partial class MenuSelectionActionsView : UserControl
    {
        public MenuSelectionActionsView()
        {
            InitializeComponent();
        }

        //private void TextMenuPopup_MouseEnter(object sender, EventArgs e)
        //{
        //    EventAggregator.PublishMouseOverTextMenuPopupChanged(true, EventArgs.Empty);
        //}

        //private void TextMenuPopup_MouseLeave(object sender, EventArgs e)
        //{
        //    EventAggregator.PublishMouseOverTextMenuPopupChanged(false, EventArgs.Empty);
        //}
    }
}
