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
using System.Windows;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.ObjectModel;
using Point = System.Drawing.Point;
using Jarvis_Windows.Sources.DataAccess.Network;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Jarvis_Windows.Sources.DataAccess;
using Jarvis_Windows.Sources.MVVM.Views.AIRead;

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
        private IAuthenticationService _authenticationService;
        private Account? _account;
        private string _username;
        private bool _isShowUsernameFirstLetter;
        private string _usernameFirstLetter;
        private string _remainingAPIUsage;
        private string _authUrl;

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

        public IAuthenticationService AuthenService
        {
            get => _authenticationService;
            set => _authenticationService = value;
        }

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowUsernameFirstLetter
        {
            get => _isShowUsernameFirstLetter;
            set
            {
                _isShowUsernameFirstLetter = value;
                OnPropertyChanged();
            }
        }

        public string UsernameFirstLetter
        {
            get { return _usernameFirstLetter; }
            set
            {
                _usernameFirstLetter = value;
                OnPropertyChanged();
            }
        }

        public string RemainingAPIUsage
        {
            get { return _remainingAPIUsage; }
            set
            {
                _remainingAPIUsage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand NavigateCommand { get; set; }
        public RelayCommand CloseMainNavigationCommand { get; set; }
        public RelayCommand OpenJarvisWebsiteCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }
        #endregion

        public MainNavigationViewModel()
        {
            _sidebarChatWidth = /*SystemParameters.WorkArea.Width*/560;
            _sidebarChatHeight = SystemParameters.WorkArea.Height;
            _navBarColors = new ObservableCollection<MainNavigationBarColor>();
            _navButtonColors = new ObservableCollection<MainNavigationFillColor>();
            _authenticationService = DependencyInjection.GetService<IAuthenticationService>();

            NavigateCommand = new RelayCommand(OnNavigate, o => true);
            CloseMainNavigationCommand = new RelayCommand(ExecuteCloseMainNavigationCommand, o => true);
            OpenJarvisWebsiteCommand = new RelayCommand(ExecuteOpenJarvisWebsiteCommand, o => true);
            LoginCommand = new RelayCommand(ExecuteLoginCommand, o => true);

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
            _makeSidebarTopmost = true;
            _authUrl = DataConfiguration.AuthUrl;

            IsShowAIChatBubble = false;
            IsShowMainNavigation = true;


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

            Account = new Account();
            Account.Username = WindowLocalStorage.ReadLocalStorage("Username");
            Account.Email = WindowLocalStorage.ReadLocalStorage("Email");
            Account.Role = WindowLocalStorage.ReadLocalStorage("Role");

            if (AuthenticationService.AuthenState == AUTHEN_STATE.NOT_AUTHENTICATED
                && Account != null)
            {
                AuthenService.SignOut();
                Account.Username = "Login";
                Account.Role = "anonymous";
                Account.Email = "example@gmail.com";
                IsShowUsernameFirstLetter = false;
            }
            else
            {
                IsShowUsernameFirstLetter = true;
            }

            Username = Account.Username;
            if (IsShowUsernameFirstLetter) UsernameFirstLetter = Username[0].ToString();
            if (string.IsNullOrEmpty(UsernameFirstLetter)) UsernameFirstLetter = "A";
        }

        private void OnPropertyMessageChanged(object sender, EventArgs e)
        {
            PropertyMessage message = (PropertyMessage)sender;
            string[] content = message.PropertyName.Split("-");

            message.PropertyName = content[0];
            string page = (content.Length > 1) ? content[1] : "";
            bool value = (bool)message.Value;
            if (message == null) return;

            switch (message.PropertyName)
            {
                case "IsShowAIChatBubble":
                    IsShowAIChatBubble = value;
                    break;
                case "IsShowMainNavigation":
                    IsShowMainNavigation = value;
                    IsShowAIChatBubble = !IsShowMainNavigation;
                    if (!string.IsNullOrEmpty(page))
                    {
                        NavigateCommand.Execute(page);
        }
                    break;
            }
        }

        private void OnNavigate(object obj)
        {
            System.Windows.Controls.Button? pressedButton = obj as System.Windows.Controls.Button;
            if (pressedButton != null)
            {
                string token = "btnNavigate";
                string targetViewModel = pressedButton.Name.ToString().Substring(token.Length);

                if (_viewModels.ContainsKey(targetViewModel))
                    CurrentViewModel = _viewModels[targetViewModel];

                ChangeNavColor(targetViewModel);
            }
            else if (obj is string)
            {
                string targetViewModel = (string)obj;
                if (_viewModels.ContainsKey(targetViewModel))
                    CurrentViewModel = _viewModels[targetViewModel];
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

        public async void ExecuteLoginCommand(object obj)
        {
            try
            {
                if (Account.Role != "anonymous")
                {
                    //EventAggregator.PublishSettingVisibilityChanged(true, EventArgs.Empty);
                    return;
                }

                string websiteUrl = _authUrl;
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = websiteUrl,
                    UseShellExecute = true
                });

            }
            catch (Exception)
            { }
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
                EventAggregator.PublishPropertyMessageChanged(new PropertyMessage("IsShowMainNavigation", !IsShowMainNavigation), new EventArgs());
                e.Handled = true;
            }
        }
    }
}
