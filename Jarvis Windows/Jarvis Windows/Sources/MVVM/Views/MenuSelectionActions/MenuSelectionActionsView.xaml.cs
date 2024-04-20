using Hardcodet.Wpf.TaskbarNotification;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void MenuSelectionActions_MouseEnter(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverActions(true, EventArgs.Empty);
        }

        private void MenuSelectionActions_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverActions(false, EventArgs.Empty);
        }
    }
}
