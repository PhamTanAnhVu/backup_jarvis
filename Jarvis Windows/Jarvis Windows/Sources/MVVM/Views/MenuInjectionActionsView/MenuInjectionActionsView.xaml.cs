using System.Windows;
using System.Windows.Input;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView
{
    public partial class MenuInjectionActionsView : Window
    {
        public MenuInjectionActionsView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void languageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void Jarvis_Custom_Action_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
