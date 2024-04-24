using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.DataAccess.Local;
using Jarvis_Windows.Sources.DataAccess;
using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.DataAccess;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.MVVM.Views.InjectionAction;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Navigation;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView
{
public class MenuInjectionActionsViewModel : ViewModelBase
{
        private string? _remainingAPIUsage;
        private string? _mainWindowInputText;
    private string _filterText;
        private bool? _isTextEmpty;
    private bool isExpanded;
        private double? _scrollBarHeight;
        private ObservableCollection<AIButton> _fixedButtons;
        private ObservableCollection<AIButton> _dynamicButtons;
    private static IAutomationElementValueService? _automationElementValueService;
        private IAuthenticationService? _authenticationService;
        private int? _languageSelectedIndex;
        private bool? _isAPIUsageRemain;
        private bool? _isOutOfToken;
        private bool? _isActionTranslate;
        private int? _previousCommandIdx;
    private string _authUrl;
        private Visibility _exhaustedGridVisibility;
        private double _horizontalOffset;
        private double _verticalOffset;
        public List<Language>? Languages { get; set; }
    public RelayCommand ShowMenuOperationsCommand { get; set; }
    public RelayCommand HideMenuOperationsCommand { get; set; }
    public RelayCommand AICommand { get; set; }
    public RelayCommand ExpandCommand { get; set; }
    public RelayCommand OpenSettingsCommand { get; set; }
    public RelayCommand QuitAppCommand { get; set; }
    public RelayCommand PinJarvisButtonCommand { get; set; }
    public RelayCommand UndoCommand { get; set; }
    public RelayCommand RedoCommand { get; set; }
    public RelayCommand UpgradePlanCommand { get; set; }
        public RelayCommand? CopyToClipboardCommand { get; set; }
        public RelayCommand? CloseOutOfTokenPopupCommand { get; set; }

        public string? RemainingAPIUsage
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
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

    public bool IsSpinningJarvisIcon
    {
        get { return _isSpinningJarvisIcon; }
        set
        {
            _isSpinningJarvisIcon = value;
            OnPropertyChanged();
        }
    }

    public bool IsMainWindowInputTextEmpty
    {
        get
        {
            if (string.IsNullOrWhiteSpace(MainWindowInputText)) _isMainWindowInputTextEmpty = true;
            else _isMainWindowInputTextEmpty = false;
            return _isMainWindowInputTextEmpty;
        }
        set
        {
            _isMainWindowInputTextEmpty = value;
            OnPropertyChanged();
        }
    }

    public string MainWindowInputText
    {
        get { return _mainWindowInputText; }
        set
        {
            _mainWindowInputText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsMainWindowInputTextEmpty));
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

        public ObservableCollection<AIButton> FixedButtons
    {
        get { return _sendEventGA4; }
        set
        {
            _sendEventGA4 = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<AIButton> FixedButtons
    {
        get { return _fixedButtons; }
        set
        {
            _fixedButtons = value;
            OnPropertyChanged();
        }
    }

        public ObservableCollection<AIButton> DynamicButtons
    {
        get { return _dynamicButtons; }
        set
        {
            _dynamicButtons = value;
            OnPropertyChanged();
        }
    }

    public string FilterText
    {
        get { return _filterText; }
        set
        {
            _filterText = value;
            UpdateButtonVisibility();
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTextEmpty));
        }
    }

        public bool? IsTextEmpty
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FilterText)) _isTextEmpty = true;
            else _isTextEmpty = false;
            return _isTextEmpty;
        }
        set
        {
            _isTextEmpty = value;
            OnPropertyChanged();
        }
    }

        public double? ScrollBarHeight
    {
        get { return _scrollBarHeight; }
        set
        {
            _scrollBarHeight = value;
            OnPropertyChanged();
        }
    }

        public int? LanguageSelectedIndex
    {
        get { return _textMenuButtons; }
        set
        {
            _textMenuButtons = value;
            OnPropertyChanged();
        }
    }
    public int LanguageSelectedIndex
    {
        get { return _languageSelectedIndex; }
        set
        {
            if (_languageSelectedIndex != value)
            {
                _languageSelectedIndex = value;
                OnPropertyChanged();
            }
        }
    }

        public double HorizontalOffset
    {
            get => _horizontalOffset;
        set
        {
                _horizontalOffset = value;
                Debug.WriteLine("HorizontalOffset = " + HorizontalOffset);
            OnPropertyChanged();
        }
    }

        public double VerticalOffset
    {
            get => _verticalOffset;
        set
        {
                _verticalOffset = value;
                Debug.WriteLine("VerticalOffset = " + VerticalOffset);
            OnPropertyChanged();
        }
    }
        public bool? IsOutOfToken
    {
            get { return _isOutOfToken; }
        set
        {
                _isOutOfToken = value;
            OnPropertyChanged();
        }
    }

        public bool? IsActionTranslate
    {
        get { return _aIChatMessages; }
        set
        {
            _aIChatMessages = value;
            OnPropertyChanged();
        }
    }
    public bool IsTextEmptyAIChat
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AIChatMessageInput)) _isTextEmptyAIChat = true;
            else _isTextEmptyAIChat = false;
            return _isTextEmptyAIChat;
        }
        set
        {
            _isTextEmptyAIChat = value;
            OnPropertyChanged();
        }
    }

    public string AIChatMessageInput
    {
        get { return _aIChatMessageInput; }
        set
        {
            _aIChatMessageInput = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTextEmptyAIChat));
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

    public bool UsernameLogoVisibility
    {
        get { return _usernameLogoVisibility; }
        set
        {
            _usernameLogoVisibility = value;
            OnPropertyChanged();
        }
    }
    public double ChatPanel_Height
    {
        get { return _chatPanel_Height; }
        set
        {
            _chatPanel_Height = value;
            OnPropertyChanged();
        }
    }

    public bool IsAPIUsageRemain
    {
        get { return _isAPIUsageRemain; }
        set
        {
            _isAPIUsageRemain = value;
            if (_isAPIUsageRemain)
            {
                ChatPanel_Height = 518;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(ChatPanel_Height));
        }
    }

    public bool IsNoAPIUsageRemain
    {
        get { return _isNoAPIUsageRemain; }
        set
        {
            _isNoAPIUsageRemain = value;
            if (_isNoAPIUsageRemain)
            {
                ChatPanel_Height = 240;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(ChatPanel_Height));
        }
    }

    public string TextMenuPinColor
    {
        get { return _textMenuPinColor; }
        set
        {
            _textMenuPinColor = value;
            OnPropertyChanged();
        }
    }

    public string TextMenuAPIHeaderActionName
    {
        get { return _textMenuAPIHeaderActionName; }
        set
        {
            _textMenuAPIHeaderActionName = value;
            OnPropertyChanged();
        }
    }

    public bool IsTextMenuAPIHeaderAction
    {
        get { return _isTextMenuAPIHeaderAction; }
        set
        {
            _isTextMenuAPIHeaderAction = value;
            OnPropertyChanged();
        }
    }
    public bool IsActionTranslate
    {
        get { return _isActionTranslate; }
        set
        {
            _isActionTranslate = value;
            OnPropertyChanged();
        }
    }

        public int? PreviousCommandIdx
    {
        get { return _previousCommandIdx; }
        set
        {
            _previousCommandIdx = value;
            OnPropertyChanged();
        }
    }

        public static IAutomationElementValueService? AutomationElementValueService
    {
        get => _automationElementValueService;
        set => _automationElementValueService = value;
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

        public IAuthenticationService? AuthenService
    {
        get => _authenticationService;
        set => _authenticationService = value;
    }

        public Visibility ExhaustedGridVisibility
    {
            get => _exhaustedGridVisibility;
        set
        {
                _exhaustedGridVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility WindowVisibility 
    { 
        get => _windowVisibility; 
        set
        {
            _windowVisibility = value;
            OnPropertyChanged();
        }
    }


    public TokenLocalService TokenLocalService
    {
        get => _tokenLocalService;
        set
        {
            _tokenLocalService = value;
            OnPropertyChanged("TokenService");
        }
    }


    public MenuInjectionActionsViewModel()
    {
            _filterText = string.Empty;
            _fixedButtons = new ObservableCollection<AIButton>();
            _dynamicButtons = new ObservableCollection<AIButton>();
        
            AutomationElementValueService = (AutomationElementValueService)DependencyInjection.GetService<IAutomationElementValueService>();
            AuthenService = (AuthenticationService)DependencyInjection.GetService<IAuthenticationService>();
            _ = ResetAPIUsageDaily();
        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
            IsOutOfToken = (RemainingAPIUsage == "0 🔥") ? true : false;

        //TEST AUTO RESET API USAGE
            //if (IsAPIUsageRemain == false)
            //{
            //    WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", "10");
            //    RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
            //    IsAPIUsageRemain = true;
            //}


        ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
        HideMenuOperationsCommand = new RelayCommand(ExecuteHideMenuOperationsCommand, o => true);

        AICommand = new RelayCommand(ExecuteAICommand, o => true);
        ExpandCommand = new RelayCommand(ExecuteExpandCommand, o => true);

        OpenSettingsCommand = new RelayCommand(ExecuteOpenSettingsCommand, o => true);
        QuitAppCommand = new RelayCommand(ExecuteQuitAppCommand, o => true);
        PinJarvisButtonCommand = new RelayCommand(ExecutePinJarvisButtonCommand, o => true);
        UndoCommand = new RelayCommand(ExecuteUndoCommand, o => true);
        RedoCommand = new RelayCommand(ExecuteRedoCommand, o => true);

        TextMenuAICommand = new RelayCommand(ExecuteTextMenuAICommand, o => true);
        ShowTextMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
        HideTextMenuAPICommand = new RelayCommand(ExecuteHideTextMenuAPICommand, o => true);

        UpgradePlanCommand = new RelayCommand(ExecuteUpgradePlanCommand, o => true);
            CloseOutOfTokenPopupCommand = new RelayCommand(o => { IsOutOfToken = false; }, o => true);

        string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = "";
        jsonContent = File.ReadAllText(fullPath);

        Languages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
        TextMenuLanguages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
        LanguageSelectedIndex = 14;
        _authUrl = DataConfiguration.AuthUrl;

        //Register Acceccibility service
        AccessibilityService.SubscribeToElementFocusChanged();
        EventAggregator.LanguageSelectionChanged += OnLanguageSelectionChanged;

        // Checking App update here
        try { ExecuteCheckUpdate(); }

        catch { }
        finally { ExecuteSendEventOpenMainWindow(); }

        try { ExecuteGetUserGeoLocation(); }
        catch { }

        InitializeButtons();
        InitializeButtonsTextMenu();

            EventAggregator.ApiUsageChanged += (sender, e) =>
        {
                RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
                IsOutOfToken = (RemainingAPIUsage == "0 🔥") ? true : false;
        };

        _globalMouseHook = Hook.GlobalEvents();
        _globalMouseHook.MouseDoubleClick += MouseDoubleClicked;
        _globalMouseHook.MouseDragFinished += MouseDragFinished;
        _globalMouseHook.MouseClick += MouseClicked;

        EventAggregator.MouseOverAppUIChanged += (sender, e) => {
            _isMouseOver_AppUI = (bool)sender;
        };

        EventAggregator.MouseOverTextSelectionMenuChanged += (sender, e) => {
            _isMouseOver_TextSelectionMenu = (bool)sender;
        };

        EventAggregator.MouseOverTextMenuPopupChanged += (sender, e) => {
            _isMouseOver_TextMenuPopup = (bool)sender;
        };
    }

        private void InitialCommands()
    {
            ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
            HideMenuOperationsCommand = new RelayCommand(ExecuteHideMenuOperationsCommand, o => true);
            AICommand = new RelayCommand(ExecuteAICommand, o => true);
            ExpandCommand = new RelayCommand(ExecuteExpandCommand, o => true);
            UndoCommand = new RelayCommand(ExecuteUndoCommand, o => true);
            RedoCommand = new RelayCommand(ExecuteRedoCommand, o => true);
            }

        private async Task ResetAPIUsageDaily()
        {
            //string dayTodayString = DateTime.Now.Day.ToString();
            //if (dayTodayString != WindowLocalStorage.ReadLocalStorage("RecentDate"))
            //{
            //    await JarvisApi.Instance.APIUsageHandler();
            //}
        }
    }

    private async Task ResetAPIUsageDaily()
    {
        await JarvisApi.Instance.APIUsageHandler();
    }

    private void UpdateButtonVisibility()
    {
        string _curFilterText = (string.IsNullOrEmpty(FilterText)) ? "" : FilterText.ToLower();
        double _currentHeight = 0;
        double _lineWidth = 0;

        foreach (var button in FixedButtons)
        {
            button.Visibility = (_curFilterText == "") || button.Content.ToLower().Contains(_curFilterText);
            button.Margin = new Thickness(0, 0, button.Visibility ? 10 : 0, button.Visibility ? 10 : 0);
            _lineWidth += (button.Visibility) ? (button.Width + 10) : 0;
        }

        if (_lineWidth > 0)
        {
            _lineWidth = 0;
            _currentHeight = 51;
        }

        foreach (var button in DynamicButtons)
        {
            int i = DynamicButtons.IndexOf(button);

            if (_curFilterText == "" && i >= 2) button.Visibility = isExpanded;
            else if (i != 1 && i < DynamicButtons.Count - 1)
                button.Visibility = button.Content.ToLower().Contains(_curFilterText);

            button.Margin = new Thickness(0, 0, button.Visibility ? 10 : 0, 10);
            _lineWidth += (button.Visibility) ? (button.Width + 10) : 0;

            if (_lineWidth > 376)
            {
                _lineWidth = button.Width + 10;
                _currentHeight += 51;
            }
        }

        if (_lineWidth > 0) { _currentHeight += 51; }

        _currentHeight = Math.Min(_currentHeight, 255);

        ScrollBarHeight = _currentHeight;

        OnPropertyChanged(nameof(FixedButtons));
        OnPropertyChanged(nameof(DynamicButtons));
    }

    private void InitializeButtons()
    {
        AIActionTemplate aIActionTemplate = new AIActionTemplate();
        DynamicButtons = aIActionTemplate.DynamicAIActionList;
        FixedButtons = aIActionTemplate.FixedAIActionList;

        foreach (var action in FixedButtons)
        {
            action.Command = new RelayCommand(ExecuteAICommand, o => true);
        }

        foreach (var action in DynamicButtons)
        {
            action.Command = (action.Content.Contains("More") || action.Content.Contains("Less"))
                ? new RelayCommand(ExecuteExpandCommand, o => true)
                : new RelayCommand(ExecuteAICommand, o => true);
        }

        UpdateButtonVisibility();
    }

        private void OnLanguageSelectionChanged(object? sender, EventArgs e)
    {
        EventAggregator.PublishSettingVisibilityChanged(true, EventArgs.Empty);
    }

    private void OnLanguageSelectionChanged(object sender, EventArgs e)
    {
        AICommand.Execute("Translate it");
    }

    private void ExecuteHideMenuOperationsCommand(object obj)
    {
            PopupDictionaryService.Instance().IsShowMenuOperations = false;
            PopupDictionaryService.Instance().ShowJarvisAction(true);
        }
    }

    private void ExecuteQuitAppCommand(object obj)
    {
        Process.GetCurrentProcess().Kill();
        Task.Run(async () =>
        {
                await Task.Delay(0);
                await GoogleAnalyticService.Instance().SendEvent("quit_app");
        });
    }

    private void ExecuteUndoCommand(object obj)
    {
            AutomationElementValueService?.Undo(AccessibilityService.GetInstance().GetFocusingElement());
        }
        catch { }
        finally
        {
        }
    }
    private void ExecuteRedoCommand(object obj)
    {
            AutomationElementValueService?.Redo(AccessibilityService.GetInstance().GetFocusingElement());
    }

    private async void ExecuteCheckUpdate()
    {
            await GoogleAnalyticService.Instance().CheckVersion();
    }
    private async void ExecuteGetUserGeoLocation()
    {
            await GoogleAnalyticService.Instance().GetUserGeoLocation();
    }

    private async void ExecuteSendEventOpenMainWindow()
    {
            await GoogleAnalyticService.Instance().SendEvent("open_main_window");
    }

        public async void ExecuteShowMenuOperationsCommand(object? obj)
            {
                await Task.Run(async () =>
                {
                await Task.Delay(0);
                await GoogleAnalyticService.Instance().SendEvent("open_input_actions");
                });
            }

        private void ExecuteExpandCommand(object parameter)
    {
        isExpanded = !isExpanded;
        DynamicButtons[1].Visibility = !isExpanded;

        for (int i = 2; i < DynamicButtons.Count; i++)
            DynamicButtons[i].Visibility = isExpanded;

        UpdateButtonVisibility();
    }

    public async void ExecuteAICommand(object obj)
    {
            if (RemainingAPIUsage == "0 🔥")
            {
                IsOutOfToken = true;
                return;
            }

        string _actionType = (string)obj;
        string _aiAction = "custom";
        try
        {
            bool _fromWindow = false;
            HideMenuOperationsCommand.Execute(null);
                InjectionActionViewModel.StartSpinJarvisIconCommand?.Execute(null);
                PopupDictionaryService.Instance().ShowJarvisAction(true);

            var textFromElement = "";
            var textFromAPI = "";
            try
            {
                    textFromElement = (String.IsNullOrEmpty(AccessibilityService.GetInstance().CurrentSelectedText)) ?
                        AccessibilityService.GetInstance().GetTextFromFocusingEditElement() :
                        AccessibilityService.GetInstance().CurrentSelectedText;
                    Debug.WriteLine($"?????? TEXT FROM ELEMENT {textFromElement}");
            }
            catch
            {
                    //textFromElement = MainWindowInputText;
                _fromWindow = true;
            }

            if (_actionType == "Translate it")
            {
                textFromAPI = await JarvisApi.Instance.TranslateHandler(textFromElement, PopupDictionaryService.TargetLangguage);
                _aiAction = "translate";
            }

            else if (_actionType == "Revise it")
            {
                textFromAPI = await JarvisApi.Instance.ReviseHandler(textFromElement);
                _aiAction = "revise";
            }
            else if (_actionType == "Ask")
            {
                textFromAPI = await JarvisApi.Instance.AskHandler(textFromElement, FilterText);
                _aiAction = "ask";
            }

            else
                textFromAPI = await JarvisApi.Instance.AIHandler(textFromElement, _actionType);

                //bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 🔥");
                //RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
                //IsAPIUsageRemain = ((RemainingAPIUsage != "0 🔥") | previousRemaingAPIUSage) ? true : false;
                //IsOutOfToken = !IsAPIUsageRemain;


            if (textFromAPI == null)
            {
                    Debug.WriteLine($"?????? {ErrorConstant.translateError}");
                return;
            }


                if (_fromWindow != true) { AccessibilityService.GetInstance().SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.translateError); }
                else { 
                    //MainWindowInputText = textFromAPI;
                }
                AutomationElementValueService?.StoreAction(AccessibilityService.GetInstance().GetFocusingElement(), textFromElement);
        }
        catch { }
        finally
        {
                InjectionActionViewModel.StopSpinJarvisIconCommand?.Execute(null);
            var eventParams = new Dictionary<string, object>
            {
                { "ai_action", _aiAction }
            };

            if (_aiAction == "translate")
                eventParams.Add("ai_action_translate_to", PopupDictionaryService.TargetLangguage);
            else if (_aiAction == "custom")
                eventParams.Add("ai_action_custom", _actionType);

                await GoogleAnalyticService.Instance().SendEvent("do_ai_action", eventParams);
    }

    private void RetrieveUserInfo()
    {
        Account.Username = WindowLocalStorage.ReadLocalStorage("Username");
        Account.Email = WindowLocalStorage.ReadLocalStorage("Email");
        Account.Role = WindowLocalStorage.ReadLocalStorage("Role");
        Username = (Account.Role == "anonymous") ? "Login" : Account.Username;
        UsernameFirstLetter = (Account.Role != "anonymous") ? Username[0].ToString() : "";
        bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 ??");
        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} ??";
        IsAPIUsageRemain = ((RemainingAPIUsage != "0 ??") | previousRemaingAPIUSage) ? true : false;
        IsNoAPIUsageRemain = !IsAPIUsageRemain;
    }
        public void ExecuteUpgradePlanCommand(object obj)
            {
                EventAggregator.PublishSettingVisibilityChanged(true, EventArgs.Empty);
                return;
            }

            //await AuthenService.SignOut();
            //await AuthenService.SignIn("aa@gmail.com", "vudet11Q");
            //string token = WindowLocalStorage.ReadLocalStorage("access_token");
            //Account = await AuthenService.GetMe();
            //UsernameFirstLetter = Account.Username[0].ToString();
            //Username = Account.Username;
            //_ = await JarvisApi.Instance.APIUsageHandler();
            //bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 ??");
            //RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} ??";
            //IsAPIUsageRemain = ((RemainingAPIUsage != "0 ??") | previousRemaingAPIUSage) ? true : false;
            //IsNoAPIUsageRemain = !IsAPIUsageRemain;
            //EventAggregator.PublishLoginStatusChanged("SettingWindow", EventArgs.Empty);

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

    private async void ExecuteLogoutCommand(object obj)
    {
        Account.Email = "";
        Account.Role = "anonymous";
        Account.Username = "Login";
        UsernameFirstLetter = "";

        await AuthenService.SignOut();
        await JarvisApi.Instance.APIUsageHandler();

        bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 ??");
        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} ??";
        IsAPIUsageRemain = ((RemainingAPIUsage != "0 ??") | previousRemaingAPIUSage) ? true : false;
        IsNoAPIUsageRemain = !IsAPIUsageRemain;


        EventAggregator.PublishLoginStatusChanged("SettingWindow", EventArgs.Empty);
    }

    public async void ExecuteShowAIChatSidebarCommand(object obj)
    {
        PopupDictionaryService.ShowAIChatSidebar(true);
        PopupDictionaryService.ShowAIChatBubble(false);
        await SendEventGA4.SendEvent("open_sidebar_chat");
    }

    private async void ExecuteHideAIChatSidebarCommand(object obj)
    {
        PopupDictionaryService.ShowAIChatSidebar(false);
        PopupDictionaryService.ShowAIChatBubble(true);
        // AIChatMessagesClear();
    }

    void AIChatMessagesClear()
    {
        AIChatMessages.Clear();
        AIChatMessages.Add(new AIChatMessage
        {
            // ImageSource = "../../../../Assets/Images/jarvis_logo.png",
            IsUser = false,
            IsJarvis = true,
            Message = "Hi, I am Jarvis, your powerful AI assistant. How can I help you?",
            IsLoading = false,
            IsBorderVisible = true
        });
    }

    private async void ExecuteAIChatSendCommand(object obj)
    {
        if (string.IsNullOrEmpty(AIChatMessageInput) || _isExecutingAIChatMessage || RemainingAPIUsage == "0 ??") return;

        _isExecutingAIChatMessage = true;
        AIChatMessages.Add(new AIChatMessage
        {
            // ImageSource = "../../../../Assets/Images/pencil.png",
            IsUser = true,
            IsJarvis = false,
            Message = AIChatMessageInput,
            IsLoading = false,
            IsBorderVisible = false
        });

        string tmpMessage = AIChatMessageInput;

        AIChatMessages.Add(new AIChatMessage
        {
            // ImageSource = "../../../../Assets/Images/jarvis_logo.png",
            IsUser = false,
            IsJarvis = true,
            Message = "",
            IsLoading = true,
            IsBorderVisible = true
        });


        AIChatMessageInput = "";

        int lastIndex = AIChatMessages.Count - 1;
        string responseMessage = await JarvisApi.Instance.ChatHandler(tmpMessage, AIChatMessages);

        AIChatMessages.RemoveAt(lastIndex);
        AIChatMessages.Add(new AIChatMessage
        {
            // ImageSource = "../../../../Assets/Images/jarvis_logo.png",
            IsUser = false,
            IsJarvis = true,
            Message = responseMessage,
            IsLoading = false,
            IsBorderVisible = true
        });

        bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 ??");
        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} ??";
        IsAPIUsageRemain = ((RemainingAPIUsage != "0 ??") | previousRemaingAPIUSage) ? true : false;
        IsNoAPIUsageRemain = !IsAPIUsageRemain;
        _isExecutingAIChatMessage = false;

        await SendEventGA4.SendEvent("send_chat_message");
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

        try
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                UIElementDetector.CurrentSelectedText = text;
                if (PopupDictionaryService.IsPinMenuSelectionResponse && PopupDictionaryService.IsShowMenuSelectionResponse)
                {
                    PopupDictionaryService.ShowMenuSelectionActions(false);
                    return;
                }
                PopupDictionaryService.ShowMenuSelectionActions(true);
                await SendEventGA4.SendEvent("inject_selection_actions");
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }
        }
        catch { }
    }

    private async Task<IDataObject?> RetryGetClipboardObject()
    {
        IDataObject? tmpClipboard = null;
        int retries = 10;
        while (retries > 0)
        {
            try
            {
                tmpClipboard = System.Windows.Clipboard.GetDataObject();
                break;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                if (retries == 0)
                {
                    // rethrow the exception if no retries are left
                    throw;
                }
                // wait for some time and retry
                await Task.Delay(0);
                retries--;
            }
        }
        return tmpClipboard;
    }

    static System.Windows.Point _lastMousePoint = new System.Windows.Point();
    private async void MouseDragFinished(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (_isMouseOver_AppUI)
        {
            PopupDictionaryService.ShowMenuSelectionActions(false);
            return;
        }

        System.Windows.Point currentMousePoint = new System.Windows.Point(e.X, e.Y);
        double distance = (currentMousePoint - _lastMousePoint).Length;
        if (distance < 5)
        {
            _lastMousePoint = currentMousePoint;
            return;
        }
        _lastMousePoint = currentMousePoint;

        IDataObject? tmpClipboard = RetryGetClipboardObject().Result;
        if (tmpClipboard == null) return;
        System.Windows.Clipboard.Clear();

        await Task.Delay(50);
        System.Windows.Forms.SendKeys.SendWait("^c");

        try
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                UIElementDetector.CurrentSelectedText = Clipboard.GetText();

                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double xScale = screenWidth / 1920;
                double yScale = screenHeight / 1080;
                System.Drawing.Point lpPoint;
                NativeUser32API.GetCursorPos(out lpPoint);
                Point selectedTextPosition = new Point((int)(lpPoint.X * xScale), (int)(lpPoint.Y * yScale));
                PopupDictionaryService.MenuSelectionActionsPosition = new Point(selectedTextPosition.X, selectedTextPosition.Y + 10);
                Point newPosition = new Point(selectedTextPosition.X, selectedTextPosition.Y + 50);
                PopupDictionaryService.MenuSelectionPopupListPosition = new Point(newPosition.X, newPosition.Y);
                if (!PopupDictionaryService.IsPinMenuSelectionResponse)
                {
                    PopupDictionaryService.ShowSelectionResponseView(false);
                    PopupDictionaryService.MenuSelectionResponsePosition = new Point(newPosition.X, newPosition.Y);
                }

                PopupDictionaryService.ShowMenuSelectionActions(true);
                await SendEventGA4.SendEvent("inject_selection_actions");
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }
        }
        catch
        {
            PopupDictionaryService.ShowMenuSelectionActions(false);
            PopupDictionaryService.ShowSelectionResponseView(false);
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (!string.IsNullOrEmpty(propertyName) && propertyName.Equals("TokenService"))
        {
            Account = AuthenService.GetMe().Result;
            Username = Account.Username;
            UsernameFirstLetter = Account.Username[0].ToString();
            bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 🔥");
            RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
                IsOutOfToken = ((RemainingAPIUsage == "0 🔥") | previousRemaingAPIUSage) ? true : false;
            }
        }
    }
}
