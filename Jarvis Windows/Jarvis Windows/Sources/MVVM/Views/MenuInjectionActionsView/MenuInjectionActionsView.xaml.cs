using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Windows.Automation;
using System.Windows.Interop;
using Windows.ApplicationModel.DataTransfer.DragDrop;
using Jarvis_Windows.Sources.Utils.Accessibility;
using System.Text;
using System.Runtime.InteropServices;
using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;
using System.Windows.Controls.Primitives;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView
{
    public partial class MenuInjectionActionsView : UserControl
    {
        private readonly IKeyboardMouseEvents _globalHook;
        private int _languageSelectedIndex = 14;
        private bool _isInit = false;
        private bool _isWindowClosed = false;
        private bool _isMouseOverOutOfTokenPopup;
        public MenuInjectionActionsView()
        {
            InitializeComponent();
            _globalHook = Hook.GlobalEvents();
            _globalHook.MouseClick += Global_MouseClick;

            languageComboBox.Loaded += (sender, e) =>
            {
                if (languageComboBox.Items.Count > 0)
                {
                    languageComboBox.SelectedIndex = _languageSelectedIndex;
                }
            };

        }

        private void Global_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!_isMouseOverOutOfTokenPopup)
            {
                OutOfTokenPopup.IsOpen = false;
            }
        }

        private void languageComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    PopupDictionaryService.TargetLangguage = ((Language)comboBox.SelectedItem).Value;
                    _languageSelectedIndex = comboBox.SelectedIndex;

                    comboBox.IsDropDownOpen = false;
                    if (_isInit)
                        EventAggregator.PublishLanguageSelectionChanged(this, EventArgs.Empty);
                    
                    _isInit = true;
                }
            }
        }

        private void MenuInjectionInputTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

        private void OutOfTokenPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverOutOfTokenPopup = true;
        }

        private void OutOfTokenPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverOutOfTokenPopup = false;
        }

        private void MenuInjectionInputTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Redirect the mouse wheel event to the ScrollViewer
            MenuInjectionInputTextBox.ScrollToVerticalOffset(MenuInjectionInputTextBox.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void MenuInjectionInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Scroll to the position of the caret
            ScrollToCaretPosition();
        }

        private void ScrollToCaretPosition()
        {
            // Get the position of the caret (cursor)
            var caretIndex = MenuInjectionInputTextBox.CaretIndex;
            var caretRect = MenuInjectionInputTextBox.GetRectFromCharacterIndex(caretIndex, true);

            // Scroll the ScrollViewer to the caret position
            if (caretRect != Rect.Empty)
            {
                MenuInjectionInputTextBox.ScrollToVerticalOffset(MenuInjectionInputTextBox.VerticalOffset + caretRect.Top);
            }
        }
    }
}
