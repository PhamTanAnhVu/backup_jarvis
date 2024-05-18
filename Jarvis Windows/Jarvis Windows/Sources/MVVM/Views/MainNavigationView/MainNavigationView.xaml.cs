using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;

namespace Jarvis_Windows.Sources.MVVM.Views.MainNavigationView
{
    public partial class MainNavigationView : Window
    {
        //public PopupDictionaryService PopupServiceInstance => PopupDictionaryService.Instance();
        public MainNavigationView()
        {
            InitializeComponent();

            //Startup postion
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;
            EventAggregator.PropertyMessageChanged += MainNavigationView_Activated;
        }

        private void MainNavigationView_Activated(object sender, System.EventArgs e)
        {
            PropertyMessage message = (PropertyMessage)sender;
            // Bring the window to the front when it's activated
            if (message.PropertyName == "IsShowMainNavigation" && (bool)message.Value == true)
            {
                Activate();
            }
        }
    }
}
