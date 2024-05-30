using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public partial class ReportPopupView : UserControl
{
    private readonly IKeyboardMouseEvents _globalHook;
    private bool _isMouseEnterSelectReportButton;
    public ReportPopupView()
    {
        InitializeComponent();
    }
    private void FeedbackInputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        FeedbackInputTextBox.ScrollToVerticalOffset(FeedbackInputTextBox.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void FeedbackInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ScrollToCaretPosition();
    }

    private void ScrollToCaretPosition()
    {
        var caretIndex = FeedbackInputTextBox.CaretIndex;
        var caretRect = FeedbackInputTextBox.GetRectFromCharacterIndex(caretIndex, true);

        if (caretRect != Rect.Empty)
        {
            FeedbackInputTextBox.ScrollToVerticalOffset(FeedbackInputTextBox.VerticalOffset + caretRect.Top);
        }
    }

    private void ReportBorder_PreviewMouseDown(object sender, MouseEventArgs e)
    {
        if (!_isMouseEnterSelectReportButton)
        {
            SelectReportPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false);
        }

    }
    private void SelectReportButton_MouseEnter(object sender, MouseEventArgs e)
    {
        _isMouseEnterSelectReportButton = true;
    }

    private void SelectReportButton_MouseLeave(object sender, MouseEventArgs e)
    {
        _isMouseEnterSelectReportButton = false;
    }
}
