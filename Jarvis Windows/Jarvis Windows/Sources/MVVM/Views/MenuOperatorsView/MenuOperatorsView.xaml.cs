﻿using Jarvis_Windows.Sources.MVVM.Models;
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

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return String.Empty;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private void Jarvis_Custom_Action_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Logging.Log($"Jarvis_Custom_Action_TextBox_PreviewMouseDown: Bring to front");
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
