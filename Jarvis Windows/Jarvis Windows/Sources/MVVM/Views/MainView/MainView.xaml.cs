//using Jarvis_Windows.Sources.Utils.Core;
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
using System.Windows.Controls;
namespace Jarvis_Windows.Sources.MVVM.Views.MainView;
public partial class MainView : Window
{
    private bool _isMainWindowOpened;
    private NotifyIcon _notifyIcon;
    private ContextMenuStrip _contextMenuStrip;
    private SendEventGA4 _sendEventGA4;
    private bool _isDragging;
    private System.Windows.Point _startPoint;

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
        _notifyIcon = new NotifyIcon();
        _contextMenuStrip = new ContextMenuStrip();

        string relativePath = Path.Combine("Assets", "Icons", "jarvis_logo_large.ico");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        _notifyIcon.Icon = new System.Drawing.Icon(fullPath);


        _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        _contextMenuStrip.Renderer = new MyRenderer();

        _contextMenuStrip.Items.Add("Sidebar", null, Sidebar_Click);
        _contextMenuStrip.Items.Add("Settings", null, Setting_Click);
        _contextMenuStrip.Items.Add("Quit Jarvis", null, QuitMenuItem_Click);

    _notifyIcon.ContextMenuStrip = _contextMenuStrip;
    _notifyIcon.Visible = true;
    }

    private System.Windows.Point _menuActionPoint;
    private System.Windows.Point _jarvisButtonPoint;

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
                System.Windows.Point AbsolutePos = new System.Windows.Point(
                    relative.X + jarvisActionPopup.HorizontalOffset,
                    relative.Y + jarvisActionPopup.VerticalOffset);
                jarvisActionPopup.VerticalOffset = AbsolutePos.Y - _jarvisButtonPoint.Y;
                jarvisActionPopup.HorizontalOffset = AbsolutePos.X - _jarvisButtonPoint.X;

                jarvisMenuPopup.VerticalOffset = AbsolutePos.Y - _jarvisButtonPoint.Y;
                jarvisMenuPopup.HorizontalOffset = AbsolutePos.X - _jarvisButtonPoint.X;
            }
        }
    }

    private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(null);
    }

    private void MenuAction_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _menuActionPoint = e.GetPosition(null);
    }

    private void MenuAction_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            System.Windows.Point relative = e.GetPosition(null);
            System.Windows.Point AbsolutePos = new System.Windows.Point(relative.X + jarvisMenuPopup.HorizontalOffset, relative.Y + jarvisMenuPopup.VerticalOffset);
            jarvisMenuPopup.VerticalOffset = AbsolutePos.Y - _menuActionPoint.Y;
            jarvisMenuPopup.HorizontalOffset = AbsolutePos.X - _menuActionPoint.X;
        }
    }

    private void Window_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            System.Windows.Point relative = e.GetPosition(null);
            System.Windows.Point AbsolutePos = new System.Windows.Point(relative.X + this.Left, relative.Y + this.Top);
            this.Top = AbsolutePos.Y - _startPoint.Y;
            this.Left = AbsolutePos.X - _startPoint.X;
        }
    }

    private void JarvisButton_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        jarvisActionPopup.HorizontalOffset = PopupDictionaryService.JarvisActionPosition.X;
        jarvisActionPopup.VerticalOffset = PopupDictionaryService.JarvisActionPosition.Y;
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

    private async void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            this.Show();
            this.Activate();
            if (_isMainWindowOpened == false)
                await SendEventGA4.SendEvent("open_main_window");

                _isMainWindowOpened = true;
        }
    }

    private async void Sidebar_Click(object sender, EventArgs e)
    {
        EventAggregator.PublishAIChatBubbleStatusChanged(this, EventArgs.Empty);
    }
    private async void Setting_Click(object sender, EventArgs e)
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

    private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        // Kiểm tra nút chuột trái đã được bấm để bắt đầu quá trình kéo và thả
        if (_isDragging)
        {
            System.Windows.Point mousePos = e.GetPosition(this);

            // Di chuyển UserControl theo sự chênh lệch của chuột
            double newLeft = mousePos.X - _startPoint.X;
            double newTop = mousePos.Y - _startPoint.Y;
            jarvisActionPopup.SetValue(Canvas.LeftProperty, newLeft);
            jarvisActionPopup.SetValue(Canvas.TopProperty, newTop);

            _startPoint = mousePos;
        }
    }

    private void draggableUserControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Bắt đầu quá trình kéo và thả
        _isDragging = true;
        _startPoint = e.GetPosition(this);
    }

    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    {
        // Kết thúc quá trình kéo và thả
        _isDragging = false;
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
}
