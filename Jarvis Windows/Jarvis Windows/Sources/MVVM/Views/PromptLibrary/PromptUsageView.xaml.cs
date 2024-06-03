using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.MVVM.Views.PromptLibrary;

/// <summary>
/// Interaction logic for PromptUsageView.xaml
/// </summary>
public partial class PromptUsageView : UserControl
{
    private bool _isMouseOverOutputLanguageButton;
    public PromptUsageView()
    {
        InitializeComponent();
    }
    
    private void SelectButton_MouseEnter(object sender, MouseEventArgs e)
    {
        Button button = sender as Button;
        string tag = (string)button.Tag;
        if (tag == "OutputLanguage") { _isMouseOverOutputLanguageButton = true; }
    }

    private void SelectButton_MouseLeave(object sender, MouseEventArgs e)
    {
        Button button = sender as Button;
        string tag = (string)button.Tag;
        if (tag == "OutputLanguage") { _isMouseOverOutputLanguageButton = false; }
    }

    private void InputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        textBox.ScrollToVerticalOffset(textBox.VerticalOffset - e.Delta); 
        e.Handled = true;
    }

    private void InputTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
