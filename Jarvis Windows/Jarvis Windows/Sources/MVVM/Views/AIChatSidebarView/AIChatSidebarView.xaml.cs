using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Windows.Automation;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using Gma.System.MouseKeyHook;
using Windows.Foundation.Diagnostics;
using System.IO;
using Jarvis_Windows.Sources.MVVM.Models;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit;
using Jarvis_Windows.Sources.MVVM.Views.AIRead;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Windows.Documents;
using System.Windows.Media;
using Windows.Security.Authentication.Identity.Provider;
using System.Globalization;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public partial class AIChatSidebarView : UserControl
{
    private readonly IKeyboardMouseEvents _globalHook;
    private bool _isMouseOverInfoPopup;
    private bool _isMouseOverOutOfTokenPopup;
    private int _idx;
    private int _itemIdx;
    
    private TextEditor? _textEditor;
    private RichTextBox? _richTextBox;
    public AIChatSidebarView()
    {
        InitializeComponent();
        _globalHook = Hook.GlobalEvents();
        _globalHook.MouseClick += Global_MouseClick;
        _globalHook.MouseWheelExt += GlobalMouseHook_MouseWheelExt;
    }

    private void Global_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (!_isMouseOverInfoPopup)
        {
            AIChatSidebarEventTrigger.PublishMouseOverInfoPopup(true, EventArgs.Empty);
        }

        if (MainChatSidebarBorder is null) return;
        if (PresentationSource.FromVisual(MainChatSidebarBorder) == null) return;

        Point mousePosition = new Point(e.X, e.Y);
        Point SidebarPosition = MainChatSidebarBorder.PointToScreen(new Point(0, 0));
        SidebarPosition.Y += 154;

        if ((mousePosition.Y < SidebarPosition.Y || mousePosition.Y > SidebarPosition.Y + 780 || mousePosition.X < SidebarPosition.X || mousePosition.X > SidebarPosition.X + 474))
        {
            AIChatSidebarEventTrigger.PublishMouseOverHistoryPopup(true, EventArgs.Empty);
        }
        else if (!_isMouseOverOutOfTokenPopup)
        {
            OutOfTokenPopup.SetCurrentValue(Popup.IsOpenProperty, false);
        }
    }

    private void InfoPopup_MouseEnter(object sender, MouseEventArgs e)
    {
        _isMouseOverInfoPopup = true;
    }
    
    private void InfoPopup_MouseLeave(object sender, MouseEventArgs e)
    {
        _isMouseOverInfoPopup = false;
    }

    private void OutOfTokenPopup_MouseEnter(object sender, MouseEventArgs e)
    {
        _isMouseOverOutOfTokenPopup = true;
    }

    private void OutOfTokenPopup_MouseLeave(object sender, MouseEventArgs e)
    {
        _isMouseOverOutOfTokenPopup = false;
    }

    private void Item_Loaded(object sender, RoutedEventArgs e)
    {
        var itemControl = (ItemsControl)sender;    
        _itemIdx = (int)itemControl.Tag;
        
    }
    private void CodeEditor_Loaded(object sender, RoutedEventArgs e)
    {
        _textEditor = sender as TextEditor;
        _idx = (int)_textEditor.Tag;

        if (_textEditor != null)
        {
            var viewModel = DataContext as AIChatSidebarViewModel;
            if (viewModel != null)
            {
                if (_itemIdx == viewModel.AIChatMessages.Count - 1)
                {
                    MainScrollViewer.ScrollToBottom();
                }

                //if (_idx >= viewModel.AIChatMessages[_itemIdx].DetailMessage.Count) 
                //    _itemIdx = _tempItemIdx;
                string codeContent = viewModel.AIChatMessages[_itemIdx].DetailMessage[_idx].CodeContent;
                string language = viewModel.AIChatMessages[_itemIdx].DetailMessage[_idx].Language;
                if (string.IsNullOrEmpty(codeContent))
                {
                    return;
                }

                _textEditor.Text = codeContent;
                _textEditor.SetCurrentValue(TextEditor.SyntaxHighlightingProperty, HighlightingManager.Instance.GetDefinition(language));
            }
        }
    }

    private void Execute_CopyCode(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var parent = VisualTreeHelper.GetParent(button);
        while (parent != null && !(parent is StackPanel))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        if (parent == null) { return; }
        var textEditor = FindChild<TextEditor>(parent);
        if (textEditor != null) {  Clipboard.SetText(textEditor.Text); }

    }

    public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;

        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T childOfType)
            {
                return childOfType;
            }

            var childItem = FindChild<T>(child);
            if (childItem != null)
            {
                return childItem;
            }
        }

        return null;
    }


    private void TextEditor_GotFocus(object sender, RoutedEventArgs e)
    {
        _textEditor = sender as TextEditor;
        _textEditor.TextArea.Caret.CaretBrush = Brushes.Transparent;
        if (_richTextBox != null)
        {
            _richTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
        }
    }
    private void TextEditor_LostFocus(object sender, RoutedEventArgs e)
    {
        _textEditor = sender as TextEditor;
        _textEditor.TextArea.Caret.CaretBrush = Brushes.Black;
    }

    private void RichTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        _richTextBox = sender as RichTextBox;
        if (_textEditor != null)
        {
            _textEditor.TextArea.ClearSelection();
        }
    }

    private void GlobalMouseHook_MouseWheelExt(object sender, MouseEventExtArgs e)
    {
        if (!_isMouseOverCodeScrollViewer) return;
        MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
    }

    private bool _isMouseOverCodeScrollViewer;
    private void MouseEnter_CodeScrollViewer(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _isMouseOverCodeScrollViewer = true;
    }

    private void MouseLeave_CodeScrollViewer(object sender, System.Windows.Input.MouseEventArgs e)
    {
        _isMouseOverCodeScrollViewer = false;
    }


    private void AIChatInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Scroll to the position of the caret
        ScrollToCaretPosition();
    }

    private void ScrollToCaretPosition()
    {
        // Get the position of the caret (cursor)
        var caretIndex = AIChatInputTextBox.CaretIndex;
        var caretRect = AIChatInputTextBox.GetRectFromCharacterIndex(caretIndex, true);

        // Scroll the ScrollViewer to the caret position
        if (caretRect != Rect.Empty)
        {
            AIChatInputTextBox.ScrollToVerticalOffset(AIChatInputTextBox.VerticalOffset + caretRect.Top);
        }
    }

    private void AIChatInputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Redirect the mouse wheel event to the ScrollViewer
        AIChatInputTextBox.ScrollToVerticalOffset(AIChatInputTextBox.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void AIChat_Sidebar_InputTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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