using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;
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
        private bool _isBubbleClicked = false;
        private Point _bubbleClickedPosition;
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

        private void MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //AIChatBubbleEventTrigger.IsDragged = false;
            _isBubbleClicked = true;
            _bubbleClickedPosition = e.GetPosition(this);

        }
        private void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isBubbleClicked)
            {
                var currentPosition = e.GetPosition(this);
                //var offsetX = currentPosition.X - _position.X;
                var offsetY = currentPosition.Y - _bubbleClickedPosition.Y;
                if (offsetY != 0)
                {
                    AIChatBubbleEventTrigger.IsBubbleDragged = true;
                }
                //aIChatBubblePopup.HorizontalOffset += offsetX;
                aIChatBubblePopup.VerticalOffset += offsetY;

                _bubbleClickedPosition = currentPosition;
            }
        }
        private void MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isBubbleClicked = false;
            //double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            //double popupHalfWidth = aIChatBubblePopup.ActualWidth / 2;

            //if (aIChatBubblePopup.HorizontalOffset + popupHalfWidth < screenWidth / 2)
            //{
            //    aIChatBubblePopup.HorizontalOffset = 0;
            //}
            //else
            //{
            //    aIChatBubblePopup.HorizontalOffset = screenWidth - 44;
            //}
        }

        private void MainNavigationView_Activated(object? sender, System.EventArgs e)
        {
            PropertyMessage? message = (sender != null) ? (PropertyMessage)sender : null;
            // Bring the window to the front when it's activated
            if (message?.PropertyName == "IsShowMainNavigation" && (bool)message.Value == true)
            {
                //Activate();
                this.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                aIChatBubblePopup.SetCurrentValue(Popup.IsOpenProperty, false);
            }
            else if(e != null)
            {
                this.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                aIChatBubblePopup.SetCurrentValue(Popup.IsOpenProperty, true);
            }
        }
    }
}
