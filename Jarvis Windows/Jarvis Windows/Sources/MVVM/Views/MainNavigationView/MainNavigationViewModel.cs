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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Jarvis_Windows.Sources.MVVM.Views.MainNavigationView
{
    public class MainNavigationViewModel : ViewModelBase
    {
        private object _currentViewModel = new AIChatSidebarViewModel(); //Default view model
        private Dictionary<string, object> _viewModels = new Dictionary<string, object>();

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateCommand { get; set; }

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
        }

        private void OnNavigate(object obj)
        {
            Button? pressedButton = obj as Button;
            if(pressedButton != null)
            {
                string token = "btnNavigate";
                string targetViewModel = pressedButton.Name.ToString().Substring(token.Length);

                if(_viewModels.ContainsKey(targetViewModel))
                    CurrentViewModel = _viewModels[targetViewModel];
            }
        }
    }
}
