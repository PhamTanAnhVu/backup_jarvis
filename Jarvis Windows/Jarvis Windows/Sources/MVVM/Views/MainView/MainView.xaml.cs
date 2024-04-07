using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.MVVM.Views.ContextMenuView;
using Point = System.Drawing.Point;
using Windows.UI.WebUI;
using Windows.ApplicationModel.Activation;

namespace Jarvis_Windows.Sources.MVVM.Views.MainView;
public partial class MainView : Window
{
    private SendEventGA4 _sendEventGA4;
    private NotifyIcon _notifyIcon;

    private bool _isMainWindowOpened;
    private bool _isDraggingMenuOperators;
    private bool _isDraggingTextMenuAPI;
    private bool _isClickingPopup;

    private System.Windows.Point _mainWindowPoint;
    private System.Windows.Point _menuActionPoint;
    private System.Windows.Point _jarvisButtonPoint;
    private System.Windows.Point _textMenuAPIPoint;
    public SendEventGA4 SendEventGA4
    {
        get { return _sendEventGA4; }
        set { _sendEventGA4 = value; }
    }

    public PopupDictionaryService PopupDictionaryService { get; internal set; }

    public MainView()
    {
        InitializeComponent();
        InitTrayIcon();
    }

    private void InitTrayIcon()
    {
        string relativePath = Path.Combine("Assets", "Icons", "jarvis_logo_large.ico");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        _notifyIcon = new NotifyIcon();
        _notifyIcon.Icon = new Icon(fullPath);
        _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        _notifyIcon.Visible = true;
    }

    private async void NotifyIcon_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            this.Show();
            this.Activate();
            if (_isMainWindowOpened == false)
                await SendEventGA4.SendEvent("open_main_window");

            _isMainWindowOpened = true;
        }
        else if (e.Button == MouseButtons.Right)
        {
            if(_notifyIcon.ContextMenuStrip == null)
            {
                _notifyIcon.ContextMenuStrip = new CustomContextMenuView(PopupDictionaryService, SendEventGA4);
            }
        }
    }

    private void App_MouseEnter(object sender, EventArgs e)
    {
        EventAggregator.PublishMouseOverAppUIChanged(true, EventArgs.Empty);
    }

    private void App_MouseLeave(object sender, EventArgs e)
    {
        EventAggregator.PublishMouseOverAppUIChanged(false, EventArgs.Empty);
    }
    
    private async void AIChatSidebar_MouseEnter(object sender, EventArgs e)
    {
        EventAggregator.PublishMouseOverAIChatPanelChanged(true, EventArgs.Empty);
    }

    private async void AIChatSidebar_MouseLeave(object sender, EventArgs e)
    {
        EventAggregator.PublishMouseOverAIChatPanelChanged(false, EventArgs.Empty);
    }

    private void Sidebar_Click(object sender, EventArgs e)
    {
        EventAggregator.PublishAIChatBubbleStatusChanged(this, EventArgs.Empty);
    }


    private void Setting_Click(object sender, EventArgs e)
    {
        PopupDictionaryService.IsShowSettingMenu = true;
    }

    private async void QuitMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            await SendEventGA4.SendEvent("quit_app");
            Process.GetCurrentProcess().Kill(); //DaiTT fix
        }

        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message);
        }
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }

    private void btnCloseMainWindows_Click(object sender, RoutedEventArgs e)
    {
        _isMainWindowOpened = false;
        this.Hide();
    }

    private void btnMoreAtJarvis_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new ProcessStartInfo
        {
            FileName = "https://jarvis.cx/",
            UseShellExecute = true
        });
    }

    private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _mainWindowPoint = e.GetPosition(null);
    }

    private void Window_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        //if (e.LeftButton == MouseButtonState.Pressed && !_isDraggingMenuOperators && !_isDraggingTextMenuAPI && !PopupDictionaryService.IsDragging && !_isClickingPopup)
        //{
        //    System.Windows.Point relative = e.GetPosition(null);
        //    System.Windows.Point AbsolutePos = new System.Windows.Point(relative.X + this.Left, relative.Y + this.Top);
        //    //this.Top = AbsolutePos.Y - _mainWindowPoint.Y;
        //    //this.Left = AbsolutePos.X - _mainWindowPoint.X;
        //}
    }

    private void JarvisButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupDictionaryService.IsDragging = false;
        _jarvisButtonPoint = e.GetPosition(null);
    }

    private void JarvisButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            System.Windows.Point currentMousePosition = e.GetPosition(null);
            double distance = (currentMousePosition - _jarvisButtonPoint).Length;

            if (distance > 2)
            {
                PopupDictionaryService.HasPinnedJarvisButton = false;
                PopupDictionaryService.IsDragging = true;
                System.Windows.Point relative = e.GetPosition(null);
                System.Windows.Point AbsolutePos = new System.Windows.Point(relative.X + jarvisActionPopup.HorizontalOffset,relative.Y + jarvisActionPopup.VerticalOffset);
                jarvisActionPopup.VerticalOffset = AbsolutePos.Y - _jarvisButtonPoint.Y;
                jarvisActionPopup.HorizontalOffset = AbsolutePos.X - _jarvisButtonPoint.X;

                jarvisMenuPopup.VerticalOffset = AbsolutePos.Y - _jarvisButtonPoint.Y;
                jarvisMenuPopup.HorizontalOffset = AbsolutePos.X - _jarvisButtonPoint.X;
            }
        }
    }

    private void JarvisButton_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        jarvisActionPopup.HorizontalOffset = PopupDictionaryService.JarvisActionPosition.X;
        jarvisActionPopup.VerticalOffset = PopupDictionaryService.JarvisActionPosition.Y;
    }

    

    private void MenuAction_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _menuActionPoint = e.GetPosition(null);
        _isDraggingMenuOperators = true;
    }

    private void MenuAction_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            System.Windows.Point relative = e.GetPosition(null);
            System.Windows.Point AbsolutePos = new System.Windows.Point(relative.X + jarvisMenuPopup.HorizontalOffset, relative.Y + jarvisMenuPopup.VerticalOffset);

            System.Windows.Rect screenBounds = System.Windows.SystemParameters.WorkArea;
            double maxX = screenBounds.Right - jarvisMenuPopup.ActualWidth;
            double maxY = screenBounds.Bottom - jarvisMenuPopup.ActualHeight;
            double newX = Math.Min(Math.Max(AbsolutePos.X - _menuActionPoint.X, screenBounds.Left), maxX);
            double newY = Math.Min(Math.Max(AbsolutePos.Y - _menuActionPoint.Y, screenBounds.Top), maxY);

            jarvisMenuPopup.VerticalOffset = newY;
            jarvisMenuPopup.HorizontalOffset = newX;

            //jarvisMenuPopup.VerticalOffset = AbsolutePos.Y - _menuActionPoint.Y;
            //jarvisMenuPopup.HorizontalOffset = AbsolutePos.X - _menuActionPoint.X;

        }
    }

    private void MenuAction_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDraggingMenuOperators = false;
    }

    private void TextMenuAPI_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _textMenuAPIPoint = e.GetPosition(null);
        _isDraggingTextMenuAPI = true;
    }

    private void TextMenuAPI_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            System.Windows.Point relative = e.GetPosition(null);
            System.Windows.Point AbsolutePos = new System.Windows.Point(relative.X + textMenuAPIPopup.HorizontalOffset, relative.Y + textMenuAPIPopup.VerticalOffset);

            System.Windows.Rect screenBounds = System.Windows.SystemParameters.WorkArea;
            double maxX = screenBounds.Right - textMenuAPIPopup.ActualWidth;
            double maxY = screenBounds.Bottom - textMenuAPIPopup.ActualHeight;
            double newX = Math.Min(Math.Max(AbsolutePos.X - _textMenuAPIPoint.X, screenBounds.Left), maxX);
            double newY = Math.Min(Math.Max(AbsolutePos.Y - _textMenuAPIPoint.Y, screenBounds.Top), maxY);

            PopupDictionaryService.TextMenuAPIPosition = new System.Drawing.Point((int)newX, (int)newY);
        }
    }

    private void TextMenuAPI_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDraggingTextMenuAPI = false;
    }
    
    private void Popup_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isClickingPopup = true;
    }
    
    private void Popup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isClickingPopup = false;
    }

    public void PinJarvisButton()
    {
        jarvisActionPopup.HorizontalOffset = PopupDictionaryService.JarvisActionPosition.X;
        jarvisActionPopup.VerticalOffset = PopupDictionaryService.JarvisActionPosition.Y;
    }

    public void ResetBinding()
    {
        System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            BindingOperations.ClearBinding(jarvisActionPopup, Popup.VerticalOffsetProperty);
            BindingOperations.ClearBinding(jarvisActionPopup, Popup.HorizontalOffsetProperty);

            System.Windows.Data.Binding verticalBinding = new System.Windows.Data.Binding("Y");
            verticalBinding.Source = PopupDictionaryService.JarvisActionPosition;
            jarvisActionPopup.SetBinding(Popup.VerticalOffsetProperty, verticalBinding);

            System.Windows.Data.Binding horizontalBinding = new System.Windows.Data.Binding("X");
            horizontalBinding.Source = PopupDictionaryService.JarvisActionPosition;
            jarvisActionPopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBinding);


            BindingOperations.ClearBinding(jarvisMenuPopup, Popup.VerticalOffsetProperty);
            BindingOperations.ClearBinding(jarvisMenuPopup, Popup.HorizontalOffsetProperty);

            System.Windows.Data.Binding verticalBindingMenu = new System.Windows.Data.Binding("Y");
            verticalBindingMenu.Source = PopupDictionaryService.MenuOperationsPosition;
            jarvisMenuPopup.SetBinding(Popup.VerticalOffsetProperty, verticalBindingMenu);

            System.Windows.Data.Binding horizontalBindingMenu = new System.Windows.Data.Binding("X");
            horizontalBindingMenu.Source = PopupDictionaryService.MenuOperationsPosition;
            jarvisMenuPopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBindingMenu);

        }));
    }

    public void Activate(IActivatedEventArgs args)
    {
        if (this.WindowState == WindowState.Minimized)
        {
            this.WindowState = WindowState.Normal;
        }

        this.Show();
        this.Activate();
    }

    private class MyRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected) base.OnRenderMenuItemBackground(e);
            else
            {
                Rectangle rc = new Rectangle(System.Drawing.Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(System.Drawing.Brushes.Transparent, rc);
                e.Graphics.DrawRectangle(Pens.Transparent, 1, 0, rc.Width - 2, rc.Height - 1);
            }
        }
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}