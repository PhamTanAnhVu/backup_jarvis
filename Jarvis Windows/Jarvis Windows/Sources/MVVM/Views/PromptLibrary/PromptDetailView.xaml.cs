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

namespace Jarvis_Windows.Sources.MVVM.Views.PromptLibrary;

public partial class PromptDetailView : UserControl
{
    private bool _isMouseOverReportPopup;
    public PromptDetailView()
    {
        InitializeComponent();
    }
    private void ContentDetailTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ContentDetailTextBox.ScrollToVerticalOffset(ContentDetailTextBox.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void ReportPopup_MouseEnter(object sender, MouseEventArgs e)
    {
        _isMouseOverReportPopup = true;
    }

    private void ReportPopup_MouseLeave(object sender, MouseEventArgs e)
    {
        _isMouseOverReportPopup = false;
    }
}
