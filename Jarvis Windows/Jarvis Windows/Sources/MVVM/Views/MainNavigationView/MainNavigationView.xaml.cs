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
        public MainNavigationView()
        {
            InitializeComponent();

            this.Width = 474 + 70;
            this.Height = SystemParameters.WorkArea.Height * 0.98;
            userControlMainContent.Height = this.Height;
            userControlMainContent.Width = this.Width - 70;
            
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;

            aIChatBubblePopup.HorizontalOffset = (int)(SystemParameters.WorkArea.Width - /*aIChatBubblePopup.Width*/ 40);
            aIChatBubblePopup.VerticalOffset = (int)((SystemParameters.WorkArea.Height - /*aIChatBubblePopup.Height*/ 260) / 2);

            EventAggregator.PropertyMessageChanged += MainNavigationView_Activated;
        }

        private void MainNavigationView_Activated(object? sender, System.EventArgs e)
        {
            PropertyMessage? message = (sender != null ) ? (PropertyMessage)sender : null;
            // Bring the window to the front when it's activated
            if (message?.PropertyName == "IsShowMainNavigation" && (bool)message.Value == true)
            {
                Activate();
            }
        }
    }
}
