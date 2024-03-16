using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Windows.Automation;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public partial class AIChatSidebarView : UserControl
{
    private bool _isWindowClosed = false;
    public AIChatSidebarView()
    {
        InitializeComponent();
    }

    private void AIChat_Main_ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer && e.ExtentHeightChange > 0)
        {
            scrollViewer.ScrollToBottom();
        }
    }
    
    private void AIChat_Input_ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer && e.ExtentHeightChange > 0)
        {
            scrollViewer.ScrollToBottom();
        }
    }

    private void AI_Sidebar_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        HwndSource source = (HwndSource)PresentationSource.FromVisual(textBox);
        if (source != null)
        {
            IntPtr handle = source.Handle;
            var currentAutomation = AutomationElement.FromHandle(handle);
            NativeUser32API.SetForegroundWindow(handle);
        }
    }
}
