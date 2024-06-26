﻿using Jarvis_Windows.Sources.MVVM.Views.AIArt;
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
using System.Windows;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.ObjectModel;
using Point = System.Drawing.Point;

namespace Jarvis_Windows.Sources.MVVM.Views.MainNavigationView
{
    public class MainNavigationViewModel : ViewModelBase
    {
        #region Fields
        private object _currentViewModel; //Default view model
        private Dictionary<string, object> _viewModels = new Dictionary<string, object>();
        private Visibility _sidebarVisibility;
        private bool _makeSidebarTopmost;
        private bool _isShowAIChatBubble;
        private bool _isShowMainNavigation;
        private Point _aIChatBubblePosition;
        private IKeyboardMouseEvents _globalKeyboardHook;
        private static MainNavigationViewModel? _instance = null;
        public ObservableCollection<MainNavigationFillColor> _navButtonColors;
        public ObservableCollection<MainNavigationBarColor> _navBarColors;
        private double _sidebarChatWidth;
        private double _sidebarChatHeight;

        #endregion

        #region Properties
        public static MainNavigationViewModel Instance()
        {
            if (_instance == null)
            {
                _instance = new MainNavigationViewModel();
            }
            return _instance;
        }
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
        public bool IsShowAIChatBubble
        {
            get
            {
                _isShowAIChatBubble = PopupDictionaryService.Instance().IsShowAIChatBubble;
                return _isShowAIChatBubble;
            }
            set
            {
                _isShowAIChatBubble = PopupDictionaryService.Instance().IsShowAIChatBubble = value;
                OnPropertyChanged();
            }
        }
        public bool IsShowMainNavigation
        {
            get
            {
                _isShowMainNavigation = PopupDictionaryService.Instance().IsShowMainNavigation;
                return _isShowMainNavigation;
            }
            set
            {
                _isShowMainNavigation = PopupDictionaryService.Instance().IsShowMainNavigation = value;
                OnPropertyChanged();
            }
        }

        public Point AIChatBubblePosition
        {
            get
            {
                _aIChatBubblePosition = PopupDictionaryService.Instance().AIChatBubblePosition;
                return _aIChatBubblePosition;
            }
            set
            {
                _aIChatBubblePosition = PopupDictionaryService.Instance().AIChatBubblePosition = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<MainNavigationFillColor> NavButtonColors
        {
            get { return _navButtonColors; }
            set
            {
                _navButtonColors = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<MainNavigationBarColor> NavBarColors
        {
            get { return _navBarColors; }
            set
            {
                _navBarColors = value;
                OnPropertyChanged();
            }
        }

        public double SidebarChatWidth 
        { 
            get => _sidebarChatWidth;
            set
            {
                _sidebarChatWidth = value;
                OnPropertyChanged();
            }
        }
        public double SidebarChatHeight
        { 
            get => _sidebarChatHeight; 
            set
            {
                _sidebarChatHeight = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands
        public RelayCommand NavigateCommand { get; set; }
        public RelayCommand CloseMainNavigationCommand { get; set; }
        public RelayCommand OpenJarvisWebsiteCommand { get; set; }
        #endregion

        public MainNavigationViewModel()
        {
            _sidebarChatWidth = /*SystemParameters.WorkArea.Width*/560;
            _sidebarChatHeight = SystemParameters.WorkArea.Height;
            _navBarColors = new ObservableCollection<MainNavigationBarColor>();
            _navButtonColors = new ObservableCollection<MainNavigationFillColor>();

            NavigateCommand = new RelayCommand(OnNavigate, o => true);
            CloseMainNavigationCommand = new RelayCommand(ExecuteCloseMainNavigationCommand, o => true);
            OpenJarvisWebsiteCommand = new RelayCommand(ExecuteOpenJarvisWebsiteCommand, o => true);

            _viewModels.Add("Chat", new AIChatSidebarViewModel());
            _viewModels.Add("Read", new AIReadViewModel()); 
            _viewModels.Add("Search", new AISearchViewModel());
            _viewModels.Add("Write", new AIWriteViewModel());
            _viewModels.Add("Translate", new AITranslateViewModel());
            _viewModels.Add("Art", new AIArtViewModel());
            _viewModels.Add("MoreInfo", new MoreInfoViewModel());
            _viewModels.Add("Settings", new SettingsViewModel());
            _viewModels.Add("Profile", new ProfileViewModel());

            _currentViewModel = _viewModels["Chat"];
            _sidebarVisibility = Visibility.Visible;
            _makeSidebarTopmost = false;
            
            IsShowAIChatBubble = true;
            IsShowMainNavigation = false;


            AIChatBubblePosition = new Point();

            _globalKeyboardHook = Hook.GlobalEvents();
            _globalKeyboardHook.KeyDown += KeyboardShortcutEvents;

            EventAggregator.PropertyMessageChanged += OnPropertyMessageChanged;
            NavButtonColors = new ObservableCollection<MainNavigationFillColor>();
            NavBarColors = new ObservableCollection<MainNavigationBarColor>();
            string[] buttonNames = ["chat", "read", "search", "write", "translate", "art"];

            for (int i = 0; i < 6; i++)
            {
                MainNavigationFillColor fillColor = new MainNavigationFillColor
                {
                    C1 = "#64748B",
                    C2 = "#64748B",
                    Name = buttonNames[i],
                };

                MainNavigationBarColor barColor = new MainNavigationBarColor
                {
                    C1 = "Transparent",
                    C2 = "Transparent",
                    Name = buttonNames[i],
                };


                NavButtonColors.Add(fillColor);
                NavBarColors.Add(barColor);
            }

            NavButtonColors[0].C1 = NavBarColors[0].C1 = "#0078D4";
            NavButtonColors[0].C2 = NavBarColors[0].C2 = "#9692FF";
        }

        private void OnPropertyMessageChanged(object sender, EventArgs e)
        {
            PropertyMessage message = (PropertyMessage)sender;
            bool value = (bool)message.Value;
            if (message == null) return;

            switch (message.PropertyName)
            {
                case "IsShowAIChatBubble":
                    IsShowAIChatBubble = value;
                    break;
                case "IsShowMainNavigation":
                    IsShowMainNavigation = value;
                    IsShowAIChatBubble = false;
                    break;
            }
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

              ChangeNavColor(targetViewModel);
            }
        }

        private void ChangeNavColor(string buttonName)
        {
            for (int idx = 0; idx < NavButtonColors.Count; idx++)
            {
                NavButtonColors[idx].C1 = NavButtonColors[idx].C2 = "#64748B";
                NavBarColors[idx].C1 = NavBarColors[idx].C2 = "Transparent";
                if (buttonName.ToLower().Contains(NavButtonColors[idx].Name))
                {
                    NavButtonColors[idx].C1 = NavBarColors[idx].C1 = "#0078D4";
                    NavButtonColors[idx].C2 = NavBarColors[idx].C2 = "#9692FF";
                }
            }

            OnPropertyChanged(nameof(NavButtonColors));
            OnPropertyChanged(nameof(NavBarColors));
        }

        private async void ExecuteOpenJarvisWebsiteCommand(object obj)
        {
            string websiteUrl = (string)obj;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = websiteUrl,
                UseShellExecute = true
            });
        }

        private void ExecuteCloseMainNavigationCommand(object obj)
        {
            IsShowMainNavigation = false;
            IsShowAIChatBubble = true;
            EventAggregator.PublishPropertyMessageChanged(new PropertyMessage("IsShowMainNavigation", false), new EventArgs());
            SidebarVisibility = Visibility.Hidden;
            MakeSidebarTopmost = false;
        }

        private void KeyboardShortcutEvents(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.J)
            {
                EventAggregator.PublishPropertyMessageChanged(new PropertyMessage("IsShowMainNavigation", true), new EventArgs());
                e.Handled = true;
            }
        }
    }
}
