using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Windows.Controls;
using System;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Automation;

namespace Jarvis_Windows.Sources.MVVM.Views.TextMenuAPIView
{
    public partial class TextMenuAPIView : UserControl
    {
        public TextMenuAPIView()
        {
            InitializeComponent();
        }

        private void TextMenuAPI_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
}
