using Gma.System.MouseKeyHook;
using ICSharpCode.AvalonEdit;
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
using System.Windows.Threading;

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

        private void MenuSelectionResponse_MouseEnter(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverResponse(true, EventArgs.Empty);
        }

        private void MenuSelectionResponse_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverResponse(false, EventArgs.Empty);
        }

        private void MenuSelectionPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverPopup(true, EventArgs.Empty);
        }

        private void MenuSelectionPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuSelectionSharedData.PublishMouseOverPopup(false, EventArgs.Empty);
        }

        private void MenuSelectionResponseInputTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

        private void Execute_Copy(object sender, RoutedEventArgs e)
        {
            CopySVGIcon.SetCurrentValue(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M13.854 3.646a.5.5 0 0 1 0 .708l-7 7a.5.5 0 0 1-.708 0l-3.5-3.5a.5.5 0 1 1 .708-.708L6.5 10.293l6.646-6.647a.5.5 0 0 1 .708 0"));
            CopyPopupText.SetCurrentValue(TextBlock.TextProperty, "Copied!");

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };

            timer.Tick += (s, args) =>
            {
                CopySVGIcon.SetCurrentValue(System.Windows.Shapes.Path.DataProperty, Geometry.Parse("M4,2.5 C4,1.67157 4.67157,1 5.5,1 L11.5,1 C12.3284,1 13,1.67157 13,2.5 L13,8.5 C13,9.32843 12.3284,10 11.5,10 L5.5,10 C4.67157,10 4,9.32843 4,8.5 L4,2.5 Z M5.5,1.75 C5.08579,1.75 4.75,2.08579 4.75,2.5 L4.75,8.5 C4.75,8.91421 5.08579,9.25 5.5,9.25 L11.5,9.25 C11.9142,9.25 12.25,8.91421 12.25,8.5 L12.25,2.5 C12.25,2.08579 11.9142,1.75 11.5,1.75 L5.5,1.75 Z M2.5,4.75 C2.08579,4.75 1.75,5.08579 1.75,5.5 L1.75,11.5 C1.75,11.9142 2.08579,12.25 2.5,12.25 L8.5,12.25 C8.91421,12.25 9.25,11.9142 9.25,11.5 L10,10.75 L10,11.5 C10,12.3284 9.32843,13 8.5,13 L2.5,13 C1.67157,13 1,12.3284 1,11.5 L1,5.5 C1,4.67157 1.67157,4 2.5,4 L3.25,4.75 L2.5,4.75 Z"));       
                CopyPopupText.SetCurrentValue(TextBlock.TextProperty, "Copy");

                timer.Stop();
            };


            timer.Start();
        }
    }
}
