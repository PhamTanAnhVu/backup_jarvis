using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Jarvis_Windows.Sources.MVVM.Views.MainNavigationView
{
    public partial class MainNavigationView : Window
    {
        public MainNavigationView()
        {
            InitializeComponent();

            //Startup postion
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;
        }
    }
}
