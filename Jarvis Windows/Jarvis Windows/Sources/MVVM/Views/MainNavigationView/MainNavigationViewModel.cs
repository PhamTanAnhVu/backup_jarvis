using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.MVVM.Views.AIArt;
using Jarvis_Windows.Sources.MVVM.Views.AIRead;
using Jarvis_Windows.Sources.MVVM.Views.AISearch;
using Jarvis_Windows.Sources.MVVM.Views.AITranslate;
using Jarvis_Windows.Sources.MVVM.Views.AIWrite;
using Jarvis_Windows.Sources.MVVM.Views.MoreInfo;
using Jarvis_Windows.Sources.MVVM.Views.Profile;
using Jarvis_Windows.Sources.MVVM.Views.Settings;
using Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;
using Jarvis_Windows.Sources.Utils.Core;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace Jarvis_Windows.Sources.MVVM.Views.MainNavigationView
{
    public class MainNavigationViewModel : ViewModelBase
    {
        #region Fields
        private object _currentViewModel = new AIChatSidebarViewModel(); //Default view model
        private Dictionary<string, object> _viewModels = new Dictionary<string, object>();
        private Visibility _sidebarVisibility;
        private bool _makeSidebarTopmost;
        private IKeyboardMouseEvents _globalKeyboardHook;
        #endregion

        #region Properties
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public Visibility SidebarVisibility
        {
            get => _sidebarVisibility;
            set
            {
                _sidebarVisibility = value;
                OnPropertyChanged();
            }
        }

        public bool MakeSidebarTopmost
        {
            get => _makeSidebarTopmost;
            set
            {
                _makeSidebarTopmost = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands
        public RelayCommand NavigateCommand { get; set; }
        #endregion

        public MainNavigationViewModel()
        {
            NavigateCommand = new RelayCommand(OnNavigate, o => true);

            _viewModels.Add("Chat", new AIChatSidebarViewModel());
            _viewModels.Add("Read", new AIReadViewModel()); 
            _viewModels.Add("Search", new AISearchViewModel());
            _viewModels.Add("Write", new AIWriteViewModel());
            _viewModels.Add("Translate", new AITranslateViewModel());
            _viewModels.Add("Art", new AIArtViewModel());
            _viewModels.Add("MoreInfo", new MoreInfoViewModel());
            _viewModels.Add("Settings", new SettingsViewModel());
            _viewModels.Add("Profile", new ProfileViewModel());

            _sidebarVisibility = Visibility.Visible;
            _makeSidebarTopmost = false;

            _globalKeyboardHook = Hook.GlobalEvents();
            _globalKeyboardHook.KeyDown += KeyboardShortcutEvents;
        }

        private void OnNavigate(object obj)
        {
            System.Windows.Controls.Button? pressedButton = obj as System.Windows.Controls.Button;
            if(pressedButton != null)
            {
                string token = "btnNavigate";
                string targetViewModel = pressedButton.Name.ToString().Substring(token.Length);

                if(_viewModels.ContainsKey(targetViewModel))
                    CurrentViewModel = _viewModels[targetViewModel];
            }
        }

        private void KeyboardShortcutEvents(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.J)
            {
                SidebarVisibility = Visibility.Visible;
                MakeSidebarTopmost = true;
                e.Handled = true;
            }
        }
    }
}
