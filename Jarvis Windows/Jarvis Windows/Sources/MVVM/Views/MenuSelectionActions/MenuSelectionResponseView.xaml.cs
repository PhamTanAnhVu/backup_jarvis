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

namespace Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions
{
    /// <summary>
    /// Interaction logic for MenuSelectionResponseView.xaml
    /// </summary>
    public partial class MenuSelectionResponseView : UserControl
    {
        public MenuSelectionResponseView()
        {
            InitializeComponent();
        }

        private void MenuSelectionResponseView_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
