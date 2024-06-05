﻿using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;
using Jarvis_Windows.Sources.MVVM.Views.ContextMenuView;
using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;

using Point = System.Windows.Point;


namespace Jarvis_Windows.Sources.MVVM.Views.MainNavigationView
{
    public partial class MainNavigationView : Window
    {
        private bool _isMouseOverAuthenticationPopup;
        private bool _isMouseOverProfileButton;
        private bool _isBubbleClicked = false;
        private Point _bubbleClickedPosition;
        private NotifyIcon _notifyIcon;
        public MainNavigationView()
        {
            InitializeComponent();
            InitTrayIcon();

            this.Width = 474 + 70 + 24;
            this.Height = SystemParameters.WorkArea.Height * 0.98;
            userControlMainContent.Height = this.Height - 24;
            userControlMainContent.Width = 474;
            
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;

            aIChatBubblePopup.HorizontalOffset = (int)(SystemParameters.WorkArea.Width - /*aIChatBubblePopup.Width*/ 40);
            aIChatBubblePopup.VerticalOffset = (int)((SystemParameters.WorkArea.Height - /*aIChatBubblePopup.Height*/ 260) / 2);
        }

        private void InitTrayIcon()
        {
            string relativePath = Path.Combine("Assets", "Icons", "jarvis_logo_large.ico");
            string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new Icon(fullPath);
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenuStrip = new CustomContextMenuView();
        }

        private void NotifyIcon_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                EventAggregator.PublishPropertyMessageChanged(new PropertyMessage("IsShowMainNavigation", true), new EventArgs());
                _ = GoogleAnalyticService.Instance().SendEvent("open_main_window");
            }
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

        private void MainNavigationBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isMouseOverAuthenticationPopup && !_isMouseOverProfileButton) 
            { 
                AuthenticationViewPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false); 
            }
        }

        private void Popup_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Popup popup = sender as Popup;
            string tag = (string)popup.Tag;
            if (tag == "Authentication") { _isMouseOverAuthenticationPopup = true; }
        }

        private void Popup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Popup popup = sender as Popup;
            string tag = (string)popup.Tag;
            if (tag == "Authentication") { _isMouseOverAuthenticationPopup = false; }
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseOverProfileButton = true; 
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseOverProfileButton = false;
        }


    }
}
