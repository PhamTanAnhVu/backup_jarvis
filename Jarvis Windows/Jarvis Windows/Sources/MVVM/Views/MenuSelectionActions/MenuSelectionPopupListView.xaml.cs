using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MenuSelectionPopupMenuView.xaml
    /// </summary>
    public partial class MenuSelectionPopupListView : UserControl
    {
        public MenuSelectionPopupListView()
        {
            InitializeComponent();
        }

        private void MenuSelectionPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverPopup(true, EventArgs.Empty);
        }

        private void MenuSelectionPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverPopup(false, EventArgs.Empty);
        }
    }
}
