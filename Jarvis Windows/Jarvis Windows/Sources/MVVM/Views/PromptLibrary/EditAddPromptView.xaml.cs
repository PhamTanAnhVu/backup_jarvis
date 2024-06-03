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
public partial class EditAddPromptView : UserControl
{
    private bool _isMouseOverLanguageButton;
    private bool _isMouseOverCategoryButton;
    public EditAddPromptView()
    {
        InitializeComponent();
    }
    private void OpenJarvisWebsite(object sender, MouseEventArgs e)
    {
        string websiteUrl = "https://jarvis.cx/help/";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = websiteUrl,
            UseShellExecute = true
        });
    }
    private void InputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        textBox.ScrollToVerticalOffset(textBox.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
  
        var caretRect = textBox.GetRectFromCharacterIndex(textBox.CaretIndex, true);
        if (caretRect != Rect.Empty) textBox.ScrollToVerticalOffset(textBox.VerticalOffset + caretRect.Top);
    }

    private void EditBorder_PreviewMouseDown(object sender, MouseEventArgs e)
    {
        if (!_isMouseOverLanguageButton) { LanguagePopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false); }
        if (!_isMouseOverCategoryButton) { CategoryPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false); }
    }
    private void PublicPromptSelectButton_MouseEnter(object sender, MouseEventArgs e)
    {
        Button button = sender as Button;
        string tag = (string)button.Tag;
        if (tag == "Language") { _isMouseOverLanguageButton = true; }
        else { _isMouseOverCategoryButton = true; }
    }

    private void PublicPromptSelectButton_MouseLeave(object sender, MouseEventArgs e)
    {
        Button button = sender as Button;
        string tag = (string)button.Tag;
        if (tag == "Language") { _isMouseOverLanguageButton = false; }
        else { _isMouseOverCategoryButton = false; }
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
