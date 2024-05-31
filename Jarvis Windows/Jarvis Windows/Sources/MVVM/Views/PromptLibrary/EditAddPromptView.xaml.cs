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
    private void PromptInputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        string tag = (string)textBox.Tag;
        if (tag == "PromptDescription") { PromptDescriptionInputTextBox.ScrollToVerticalOffset(PromptDescriptionInputTextBox.VerticalOffset - e.Delta); }
        else if (tag == "PromptName") { PromptNameInputTextBox.ScrollToVerticalOffset(PromptNameInputTextBox.VerticalOffset - e.Delta); }
        else { PromptContentInputTextBox.ScrollToVerticalOffset(PromptContentInputTextBox.VerticalOffset - e.Delta); }
        
        e.Handled = true;
    }

    private void PromptInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        string tag = (string)textBox.Tag;
        if (tag == "PromptDescription")
        {
            var caretRect = PromptDescriptionInputTextBox.GetRectFromCharacterIndex(PromptDescriptionInputTextBox.CaretIndex, true);
            if (caretRect != Rect.Empty) PromptDescriptionInputTextBox.ScrollToVerticalOffset(PromptDescriptionInputTextBox.VerticalOffset + caretRect.Top);
        }

        else if (tag == "PromptName")
        {
            var caretRect = PromptNameInputTextBox.GetRectFromCharacterIndex(PromptNameInputTextBox.CaretIndex, true);
            if (caretRect != Rect.Empty) PromptNameInputTextBox.ScrollToVerticalOffset(PromptNameInputTextBox.VerticalOffset + caretRect.Top);
        }
        else
        {
            var caretRect = PromptContentInputTextBox.GetRectFromCharacterIndex(PromptContentInputTextBox.CaretIndex, true);
            if (caretRect != Rect.Empty) PromptContentInputTextBox.ScrollToVerticalOffset(PromptContentInputTextBox.VerticalOffset + caretRect.Top);
        }
    }

    private void EditBorder_PreviewMouseDown(object sender, MouseEventArgs e)
    {
        if (!_isMouseOverLanguageButton) { }
        if (!_isMouseOverCategoryButton) { }
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
