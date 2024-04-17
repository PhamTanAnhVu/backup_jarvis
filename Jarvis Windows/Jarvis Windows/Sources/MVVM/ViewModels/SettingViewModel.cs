using Jarvis_Windows.Sources.DataAccess;
using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Jarvis_Windows.Sources.MVVM.Views.SettingView;

public class SettingViewModel : ViewModelBase
{
    private INavigationService? _navigationService;
    private IAuthenticationService _authenticationService;
    private PopupDictionaryService _popupDictionaryService;
    private UIElementDetector _accessibilityService;
    private SendEventGA4 _sendEventGA4;

    private double _usageBarGreen;

    private string _usageBarCornerRadius;
    private string _authUrl;
    private string _appVersion;
    private string _labelName;
    private string _usernameFirstLetter;
    private string _username;
    private string _email;
    private string _role;
    private string _usedAPIUsage;
    private string _dailyApiUsage;

    private bool _isGeneral;
    private bool _isAccount;
    private bool _isAbout;
    private bool _isLogin;
    private bool _isLogout;
    public double DotSpeed { get; set; }
    public double UsageBarGray { get; set; }
    public StringBuilder SettingStatus { get; set; }
    public ObservableCollection<SettingButtonModel> Settings { get; set; }
    public ObservableCollection<ToggleButtonModel> ToggleButtons { get; set; }
    private List<DispatcherTimer> StopDotTimer { get; set; }
    public RelayCommand ResetSettingsCommand { get; set; }
    public RelayCommand AuthenticateCommand { get; set; }
    public RelayCommand OpenWebsiteCommand { get; set; }

    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }
    public IAuthenticationService AuthenService
    {
        get => _authenticationService;
        set => _authenticationService = value;
    }

    public PopupDictionaryService PopupDictionaryService
    {
        get { return _popupDictionaryService; }
        set
        {
            _popupDictionaryService = value;
            OnPropertyChanged();
        }
    }

    public UIElementDetector AccessibilityService
    {
        get { return _accessibilityService; }
        set
        {
            _accessibilityService = value;
            OnPropertyChanged();
        }
    }

    public SendEventGA4 SendEventGA4
    {
        get { return _sendEventGA4; }
        set
        {
            _sendEventGA4 = value;
            OnPropertyChanged();
        }
    }

    public double UsageBarGreen
    {
        get { return _usageBarGreen; }
        set
        {
            _usageBarGreen = value;
            OnPropertyChanged();
        }
    }
    public string UsageBarCornerRadius
    {
        get { return _usageBarCornerRadius; }
        set
        {
            _usageBarCornerRadius = value;
            OnPropertyChanged();
        }
    }
    
    public string AppVersion
    {
        get { return _appVersion; }
        set
        {
            _appVersion = value;
            OnPropertyChanged();
        }
    }
    
    public string LabelName
    {
        get { return _labelName; }
        set
        {
            _labelName = value;
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
    public string Username
    {
        get { return _username; }
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }
    public string Email
    {
        get { return _email; }
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }
    public string Role
    {
        get { return _role; }
        set
        {
            _role = value;
            OnPropertyChanged();
        }
    }
    public string UsedAPIUsage
    {
        get { return _usedAPIUsage; }
        set
        {
            _usedAPIUsage = value;
            OnPropertyChanged();
        }
    }
    public string DailyApiUsage
    {
        get { return _dailyApiUsage; }
        set
        {
            _dailyApiUsage = value;
            OnPropertyChanged();
        }
    }

    public bool IsGeneral
    {
        get { return _isGeneral; }
        set
        {
            _isGeneral = value;
            OnPropertyChanged();
        }
    }
    public bool IsAccount
    {
        get { return _isAccount; }
        set
        {
            _isAccount = value;
            OnPropertyChanged();
        }
    }
    public bool IsAbout
    {
        get { return _isAbout; }
        set
        {
            _isAbout = value;
            OnPropertyChanged();
        }
    }
    public bool IsLogin
    {
        get { return _isLogin; }
        set
        {
            _isLogin = value;
            IsLogout = !_isLogin;
            OnPropertyChanged();
        }
    }
    public bool IsLogout
    {
        get { return _isLogout; }
        set
        {
            _isLogout = value;
            OnPropertyChanged();
        }
    }

    public SettingViewModel(INavigationService navigationService,
        PopupDictionaryService popupDictionaryService,
        UIElementDetector accessibilityService,
        SendEventGA4 sendEventGA4,
        IAuthenticationService authenticationService)
    {
        NavigationService = navigationService;
        PopupDictionaryService = popupDictionaryService;
        AccessibilityService = accessibilityService;
        SendEventGA4 = sendEventGA4;
        AuthenService = authenticationService;

        ResetSettingsCommand = new RelayCommand(ExecuteResetSettingsCommand, o => true);
        AuthenticateCommand = new RelayCommand(ExecuteAuthenticateCommand, o => true);
        OpenWebsiteCommand = new RelayCommand(ExecuteOpenWebsiteCommand, o => true);
        
        _authUrl = DataConfiguration.AuthUrl;
        AppVersion = $"Current version: {WindowLocalStorage.ReadLocalStorage("AppVersion")}";
        UsageBarGray = 590;

        InitLabels();
        InitializeToggleButtons();

        OnLoginStatusChanged("SettingWindow", EventArgs.Empty);
        EventAggregator.LoginStatusChanged += OnLoginStatusChanged;
    }
 
    private void InitLabels()
    {
        Settings = new ObservableCollection<SettingButtonModel>();

        string[] labels = { "Account", "General", "About" };

        for (int idx = 0; idx < labels.Length; idx++)
        {
            Settings.Add(new SettingButtonModel
            {
                Idx = idx,
                LabelName = labels[idx],
                Background = "Transparent",
                Foreground = "Black",
                FontWeight = "Normal",
                Command = new RelayCommand(ExecuteLabelCommand, o => true),
            });
        }

        ExecuteLabelCommand("0");
    }

    private void ResetLabels()
    {
        IsGeneral = IsAccount = IsAbout = false;
        foreach (var setting in Settings)
        {
            setting.Background = "Transparent";
            setting.Foreground = "Black";
            setting.FontWeight = "Normal";
        }
    }

    private void ExecuteLabelCommand(object obj)
    {
        ResetLabels();

        int idx = int.Parse(obj.ToString());
        LabelName = Settings[idx].LabelName;
        Settings[idx].Background = "#E4EBF7";
        Settings[idx].Foreground = "#1450A3";
        Settings[idx].FontWeight = "Bold";

        if (idx == 0) IsAccount = true;
        else if (idx == 1) IsGeneral = true;
        else if (idx == 2) IsAbout = true;
    }

    private void ExecuteResetSettingsCommand(object obj)
    {
        PopupDictionaryService.JarvisActionVisibility = true;
        PopupDictionaryService.TextMenuSelectionVisibility = true;
        PopupDictionaryService.IsShowAIChatBubble = true;
        WindowLocalStorage.WriteLocalStorage("SettingStatus", "1111");

        for (int idx = 0; idx < ToggleButtons.Count; idx++)
        {
            ToggleButtonModel toggleButton = ToggleButtons[idx];

            toggleButton.Background = "#1450A3";
            toggleButton.DotMargin = $"17 0 0 0";
            toggleButton.IsActive = true;           
        }
    }

    private void InitializeToggleButtons()
    {
        /* Turn on/off
         * 0: Boot at startup
         * 1: Text selection
         * 2: Input action
         * 3: AI Chat
         */
        SettingStatus = new StringBuilder(WindowLocalStorage.ReadLocalStorage("SettingStatus"));
        DotSpeed = 7;

        ToggleButtons = new ObservableCollection<ToggleButtonModel>();
        StopDotTimer = new List<DispatcherTimer>();
        string[] headers = { "Boot at startup", "Detect text selection", "Detect input action", "Toggle AI chat" };
        string[] descriptions = { "Boot at startup", "Display text selection menu when selecting text", "Display Jarvis action in input textbox", "Display AI Chat on your right sidebar" };
        for (int idx = 0; idx < 4; idx++)
        {
            bool isActive = (SettingStatus[idx] == '1');
            double leftMargin = (isActive) ? 17 : 3;
            ToggleButtonModel toggleButton = new ToggleButtonModel
            {
                Idx = idx,
                Header = headers[idx],
                Description = descriptions[idx],
                ToggleButtonWidth = 30,
                ToggleButtonHeight = 15,
                DotSize = 10,
                IsActive = (isActive),
                Background = (isActive) ? "#1450A3" : "#CCCCCC",
                DotMargin = $"{leftMargin} 0 0 0",
                Command = new RelayCommand(ExecuteToggleCommand, o => true),
            };
            ToggleButtons.Add(toggleButton);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (sender, e) => Timer_Tick(sender, e, toggleButton.Idx);
            StopDotTimer.Add(timer);
        }

        PopupDictionaryService.TextMenuSelectionVisibility = ToggleButtons[1].IsActive;
        PopupDictionaryService.JarvisActionVisibility = ToggleButtons[2].IsActive;
        PopupDictionaryService.IsShowAIChatBubble = ToggleButtons[3].IsActive;     
    }

    private async void ExecuteToggleCommand(object obj)
    {
        int idx = (int)obj;
        ToggleButtons[idx].IsActive = !ToggleButtons[idx].IsActive;
        StopDotTimer[idx].Start();

        if (idx == 1)
        {
            PopupDictionaryService.TextMenuSelectionVisibility = !PopupDictionaryService.TextMenuSelectionVisibility;
        }
        else if (idx == 2)
        {
            PopupDictionaryService.JarvisActionVisibility = !PopupDictionaryService.JarvisActionVisibility;
        }

        else if (idx == 3)
        {

            PopupDictionaryService.IsShowAIChatBubble = (PopupDictionaryService.IsShowAIChatSidebar) ? false : !PopupDictionaryService.IsShowAIChatBubble;
            PopupDictionaryService.IsShowAIChatSidebar = false;
        }

        SettingStatus[idx] = (ToggleButtons[idx].IsActive) ? '1' : '0';
        WindowLocalStorage.WriteLocalStorage("SettingStatus", SettingStatus.ToString());
    }

    private void Timer_Tick(object sender, EventArgs e, int idx)
    {
        ToggleButtonModel toggleButton = ToggleButtons[idx];
        double curMarginX = double.Parse(toggleButton.DotMargin.Split(" ")[0]);
        double minMargin = 3;
        double maxMargin = toggleButton.ToggleButtonWidth - toggleButton.DotSize - minMargin;
        if (toggleButton.IsActive)
        {
            toggleButton.Background = "#1450A3";
            curMarginX += DotSpeed;
            if (curMarginX >= maxMargin)
            {
                StopDotTimer[idx].Stop();
                curMarginX = maxMargin;
            }
        }
        else
        {
            curMarginX -= DotSpeed;
            toggleButton.Background = "#CCCCCC";
            if (curMarginX <= minMargin)
            {
                StopDotTimer[idx].Stop();
                curMarginX = minMargin;
            }
        }

        toggleButton.DotMargin = $"{curMarginX} 0 0 0";
    }

    private void RetrieveUserInfo()
    {
        Username = WindowLocalStorage.ReadLocalStorage("Username");
        Email = WindowLocalStorage.ReadLocalStorage("Email");
        Role = WindowLocalStorage.ReadLocalStorage("Role");
        UsernameFirstLetter = Username[0].ToString();

        Username = (Role == "anonymous") ? "Anonymous User" : Username;
        Email = (Role == "anonymous") ? "Please login to utilize Jarvis at full capacity" : Email;

        IsLogin = (Role != "anonymous");
        IsLogout = !IsLogin;
    }

    public async void ExecuteAuthenticateCommand(object obj)
    {
        if (IsLogin)
            await ExecuteLogout();
        else
            await ExecuteLogin();
    }

    private async Task ExecuteLogin()
    {
        try
        {
            string websiteUrl = _authUrl;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = websiteUrl,
                UseShellExecute = true
            });

            //await AuthenService.SignOut();
            //await AuthenService.SignIn("aa@gmail.com", "vudet11Q");
            //string token = WindowLocalStorage.ReadLocalStorage("access_token");
            //_ = await AuthenService.GetMe();
            //_ = await JarvisApi.Instance.APIUsageHandler();
            // RetrieveUserInfo();
            // EventAggregator.PublishLoginStatusChanged("MainWindow", EventArgs.Empty);
            // EventAggregator.PublishLoginStatusChanged("SettingWindow", EventArgs.Empty);

        }
        catch (Exception ex) { }
    }

    private async Task ExecuteLogout()
    {
        try
        {   
            await AuthenService.SignOut();
            await JarvisApi.Instance.APIUsageHandler();

            DailyApiUsage = WindowLocalStorage.ReadLocalStorage("DailyApiUsage");
            EventAggregator.PublishLoginStatusChanged("MainWindow", EventArgs.Empty);
            EventAggregator.PublishLoginStatusChanged("SettingWindow", EventArgs.Empty);

            IsLogin = false;
            IsLogout = true;
        }
        catch (Exception ex) { }
    }

    private void OnLoginStatusChanged(object sender, EventArgs e)
    {
        string type = (string)sender;
        if (type != "SettingWindow") return;

        DailyApiUsage = WindowLocalStorage.ReadLocalStorage("DailyApiUsage");
        double dailyApiUsage = double.Parse(DailyApiUsage);
        double usedApiUsage = dailyApiUsage - int.Parse(WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining"));
        double usageBarJump = UsageBarGray / double.Parse(DailyApiUsage);

        RetrieveUserInfo();
        UsedAPIUsage = $"{usedApiUsage}/{dailyApiUsage} 🔥";
        UsageBarGreen = usedApiUsage * usageBarJump;
        UsageBarCornerRadius = (usedApiUsage == dailyApiUsage) ? "5 0 0 5" : "5 5 5 5";
    }

    private async void ExecuteOpenWebsiteCommand(object obj)
    {
        string websiteUrl = (string)obj;
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = websiteUrl,
            UseShellExecute = true
        });
    }
}