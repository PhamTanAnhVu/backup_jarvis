using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Windows.Automation;
using System.Windows.Interop;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView
{
    public partial class MenuOperatorsView : UserControl
    {
        private int _languageSelectedIndex = 14;
        private bool _isInit = false;
        private bool _isWindowClosed = false;
        public MenuOperatorsView()
        {
            InitializeComponent();

            languageComboBox.Loaded += (sender, e) =>
            {
                if (languageComboBox.Items.Count > 0)
                {
                    languageComboBox.SelectedIndex = _languageSelectedIndex;
                }
            };

            EventAggregator.JarvisActionPositionChanged += OnJarvisActionPositionChanged;
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

        private void OnJarvisActionPositionChanged(object sender, EventArgs e)
        {
            string objID = (string)sender;
            if (objID == "GuidelineText") _isWindowClosed = false;
            else _isWindowClosed = true;
        }

        private void Jarvis_Custom_Action_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
