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
using System.Threading;
using System.Windows.Threading;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public partial class AIChatSidebarView : UserControl
{
    private readonly IKeyboardMouseEvents _globalHook;
    private bool _isMouseOverInfoPopup;
    private bool _isMouseOverOutOfTokenPopup;
    private bool _isMouseOverHistoryPopup;
    private bool _isMouseOverEditablePopup;
    private int _idx;
    private int _itemIdx;
    private double ChatScrollViewHeight;
    private TextEditor? _textEditor;
    private RichTextBox? _richTextBox;
    public AIChatSidebarView()
    {
        InitializeComponent();
        _globalHook = Hook.GlobalEvents();
        _globalHook.MouseClick += Global_MouseClick;
        _globalHook.MouseWheelExt += GlobalMouseHook_MouseWheelExt;
        AIChatSidebarEventTrigger.MouseOverHistoryPopup += (sender, e) =>
        {
            _isMouseOverEditablePopup = (bool)sender;
        };

        MainChatSidebarBorder.Height = (SystemParameters.WorkArea.Height * 0.98) - 24;
        ChatScrollViewHeight = (MainChatSidebarBorder.Height * 0.758) - 10;
        ExtraBorder.Height = ChatScrollViewHeight;
        ChatConversationBorder.Height = ChatScrollViewHeight;
        
        // Put ChatHistory Popup in the center, then calculate the vertical offset to align it to the MainSidebar. ChatHistory Height is fixed at 780 (no problem)
        ChatHistoryPopup.VerticalOffset = MainChatSidebarBorder.Height - (MainChatSidebarBorder.Height / 2 + 390); ;

        // Use popup for overlay border because IsOpen property is instantly updated
        // Create a border gap between ChatHistory and MainBorder at the top and fill in
        //OverlayPopup.Height = MainChatSidebarBorder.Height - 780;
        //OverlayPopup.VerticalOffset = -(MainChatSidebarBorder.Height / 2 - OverlayPopup.Height / 2);

        IsLoadingConversationPopup.Height = 500;
        IsLoadingConversationPopup.Width = 400;
        IsLoadingConversationPopup.VerticalOffset = -(50 + (MainChatSidebarBorder.Height / 2 - ChatScrollViewHeight / 2));

        AIChatSidebarEventTrigger.ScrollChatToBottom += (sender, e) =>
        {
            ChatConversationScrollViewer.ScrollToBottom();
        };
    }

    private void Global_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (!_isMouseOverInfoPopup)
        {
            AIChatSidebarEventTrigger.PublishMouseOverInfoPopup(true, EventArgs.Empty);
        }

        if (!_isMouseOverOutOfTokenPopup)
        {
            OutOfTokenPopup.SetCurrentValue(Popup.IsOpenProperty, false);
        }
        
        SelectAIModelPopup.SetCurrentValue(Popup.IsOpenProperty, false);

        if (MainChatSidebarBorder is null) return;
        if (PresentationSource.FromVisual(MainChatSidebarBorder) == null) return;

        Point mousePosition = new Point(e.X, e.Y);
        Point SidebarPosition = MainChatSidebarBorder.PointToScreen(new Point(0, 0));

        var viewModel = DataContext as AIChatSidebarViewModel;
        if (!_isMouseOverHistoryPopup && mousePosition.X >= SidebarPosition.X && mousePosition.X <= SidebarPosition.X + MainChatSidebarBorder.ActualWidth
            && !_isMouseOverEditablePopup && !viewModel.ChatHistoryViewModel.IsTitleEditable && !viewModel.ChatHistoryViewModel.IsOpenDeletePopup)
        {
            ChatHistoryPopup.SetCurrentValue(Popup.IsOpenProperty, false);
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
    private void ChatHistoryPopup_MouseEnter(object sender, MouseEventArgs e)
    {
        _isMouseOverHistoryPopup = true;
    }
    private void ChatHistoryPopup_MouseLeave(object sender, MouseEventArgs e)
    {
        _isMouseOverHistoryPopup = false;
    }
    //private void Item_Loaded(object sender, RoutedEventArgs e)
    //{
    //    var itemControl = (ItemsControl)sender;    
    //    _itemIdx = (int)itemControl.Tag;
        
    //}
    //private void CodeEditor_Loaded(object sender, RoutedEventArgs e)
    //{
    //    _textEditor = sender as TextEditor;
    //    _idx = (int)_textEditor.Tag;

    //    if (_textEditor != null)
    //    {
    //        var viewModel = DataContext as AIChatSidebarViewModel;
    //        if (viewModel != null)
    //        {
    //            if (_itemIdx == viewModel.AIChatMessages.Count - 1)
    //            {
    //                ChatConversationScrollViewer.ScrollToBottom();
    //            }

    //            while (_idx >= viewModel.AIChatMessages[_itemIdx].DetailMessage.Count)
    //                _idx--;
    //            string codeContent = viewModel.AIChatMessages[_itemIdx].DetailMessage[_idx].CodeContent;
    //            string language = viewModel.AIChatMessages[_itemIdx].DetailMessage[_idx].Language;
    //            if (string.IsNullOrEmpty(codeContent))
    //            {
    //                return;
    //            }

    //            _textEditor.Text = codeContent;
    //            _textEditor.SetCurrentValue(TextEditor.SyntaxHighlightingProperty, HighlightingManager.Instance.GetDefinition(language));
    //        }
    //    }
    //}

    private void Execute_CopyCode(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var parent = VisualTreeHelper.GetParent(button);
        TextBlock? underMessagePopupText = null;
        while (parent != null && !(parent is StackPanel))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        if (parent != null)
        {
            var textEditor = FindChild<TextEditor>(parent);
            if (textEditor != null)
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(textEditor.Text);
                }
                catch { return; }
            }
        }

        var svgPathData = FindChild<System.Windows.Shapes.Path>(button);
        if (svgPathData != null)
        {
            svgPathData.SetCurrentValue(System.Windows.Shapes.Path.DataProperty, Geometry.Parse(
                    "M13.854 3.646a.5.5 0 0 1 0 .708l-7 7a.5.5 0 0 1-.708 0l-3.5-3.5a.5.5 0 1 1 .708-.708L6.5 10.293l6.646-6.647a.5.5 0 0 1 .708 0"
                )
            );
        
        }

        var viewBox = FindChild<Viewbox>(button);
        if (viewBox != null)
        {
            viewBox.SetCurrentValue(WidthProperty, (double)14);
            viewBox.SetCurrentValue(HeightProperty, (double)14);
        }

        var textBlock = FindChild<TextBlock>(button);
        if (textBlock != null)
        {
            textBlock.SetCurrentValue(TextBlock.TextProperty, "Copied!");
        }

        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2.5)
        };


        timer.Tick += (s, args) =>
        {
            svgPathData.SetCurrentValue(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M4,2.5 C4,1.67157 4.67157,1 5.5,1 L11.5,1 C12.3284,1 13,1.67157 13,2.5 L13,8.5 C13,9.32843 12.3284,10 11.5,10 L5.5,10 C4.67157,10 4,9.32843 4,8.5 L4,2.5 Z M5.5,1.75 C5.08579,1.75 4.75,2.08579 4.75,2.5 L4.75,8.5 C4.75,8.91421 5.08579,9.25 5.5,9.25 L11.5,9.25 C11.9142,9.25 12.25,8.91421 12.25,8.5 L12.25,2.5 C12.25,2.08579 11.9142,1.75 11.5,1.75 L5.5,1.75 Z M2.5,4.75 C2.08579,4.75 1.75,5.08579 1.75,5.5 L1.75,11.5 C1.75,11.9142 2.08579,12.25 2.5,12.25 L8.5,12.25 C8.91421,12.25 9.25,11.9142 9.25,11.5 L10,10.75 L10,11.5 C10,12.3284 9.32843,13 8.5,13 L2.5,13 C1.67157,13 1,12.3284 1,11.5 L1,5.5 C1,4.67157 1.67157,4 2.5,4 L3.25,4.75 L2.5,4.75 Z"));
            double size = 12;
            if (textBlock != null)
            {
                textBlock.SetCurrentValue(TextBlock.TextProperty, "Copy code");
                size = 14;
            }
            if (underMessagePopupText is not null)
            {
                underMessagePopupText.SetCurrentValue(TextBlock.TextProperty, "Copy");
            }

            viewBox.SetCurrentValue(WidthProperty, size);
            viewBox.SetCurrentValue(HeightProperty, size);


            timer.Stop();
        };


        timer.Start();
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
        ChatConversationScrollViewer.ScrollToVerticalOffset(ChatConversationScrollViewer.VerticalOffset - e.Delta);
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

    private void InputTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Decrease height of upper scrollview if input textbox height increase.
        if (e.PreviousSize.Height != e.NewSize.Height && e.PreviousSize.Height != 0)
        {
            double diffHeight = e.NewSize.Height - 72;
            ExtraBorder.SetCurrentValue(HeightProperty, ChatScrollViewHeight - diffHeight);
            ChatConversationBorder.SetCurrentValue(HeightProperty, ChatScrollViewHeight - diffHeight);
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