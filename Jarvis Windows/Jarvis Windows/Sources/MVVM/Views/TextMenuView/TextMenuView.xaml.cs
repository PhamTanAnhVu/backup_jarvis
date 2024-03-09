using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.Utils.Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Jarvis_Windows.Sources.MVVM.Views.TextMenuView
{
    public partial class TextMenuView : UserControl
    {
        private IKeyboardMouseEvents globalMouseHook;
        public TextMenuView()
        {
            InitializeComponent();

            globalMouseHook = Hook.GlobalEvents();
            globalMouseHook.MouseDoubleClick += MouseDoubleClicked;
            globalMouseHook.MouseDragFinished += MouseDragFinished;
        }

        private async void MouseDoubleClicked(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            return;
            IDataObject tmpClipboard = System.Windows.Clipboard.GetDataObject();
            System.Windows.Clipboard.Clear();

            await Task.Delay(50);

            // Send Ctrl+C, which is "copy"
            System.Windows.Forms.SendKeys.SendWait("^c");

            await Task.Delay(50);

            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                UIElementDetector.CurrentSelectedText = text;
            }
            /*else
            {
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }*/
        }

        private async void MouseDragFinished(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            IDataObject tmpClipboard = System.Windows.Clipboard.GetDataObject();
            System.Windows.Clipboard.Clear();
            await Task.Delay(50);

            // Send Ctrl+C, which is "copy"
            System.Windows.Forms.SendKeys.SendWait("^c");

            await Task.Delay(50);

            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                UIElementDetector.CurrentSelectedText = text;
            }
            /*else
            {
                // Restore the Clipboard.
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }*/
        }
    }
}
