﻿using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView;
using Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;
using Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using Jarvis_Windows.Sources.DataAccess;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Gma.System.MouseKeyHook;
using System.Windows.Controls;
using Windows.Devices.Enumeration;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Documents;
using Windows.ApplicationModel.Background;
using System.Windows.Controls.Primitives;

namespace Jarvis_Windows.Sources.MVVM.Views.MainView;

public class MainViewModel : ViewModelBase
{
    private INavigationService? _navigationService;
    private PopupDictionaryService _popupDictionaryService;
    private UIElementDetector _accessibilityService;
    private bool _isSpinningJarvisIcon; // Spinning Jarvis icon
    private string _remainingAPIUsage;
    private string _mainWindowInputText;
    private string _filterText;
    private bool _isTextEmpty;
    private bool isExpanded;
    private double _scrollBarHeight;
    private ObservableCollection<ButtonViewModel> _fixedButtons;
    private ObservableCollection<ButtonViewModel> _dynamicButtons;
    private static IAutomationElementValueService _automationElementValueService;
    private bool _isTextEmptyAIChat;
    private string _aIChatMessageInput;

    private int _languageSelectedIndex;
    private string _textMenuAPI;
    private bool _isSpinningJarvisIconTextMenu;
    private double _textMenuAPIscrollBarHeight;
    private bool _isAPIUsageRemain;
    private bool _isNoAPIUsageRemain;
    private string _textMenuPinColor;
    private string _textMenuAPIHeaderActionName;
    private bool _isTextMenuAPIHeaderAction;
    private bool _isActionTranslate;
    private int _previousCommandIdx;

    private string _usernameFirstLetter;
    private string _username;
    private bool _isMainWindowInputTextEmpty;
    private ObservableCollection<ButtonViewModel> _textMenuButtons;
    private IKeyboardMouseEvents _globalMouseHook;
    public List<Language> TextMenuLanguages { get; set; }
    public RelayCommand TextMenuAICommand { get; set; }
    public RelayCommand ShowTextMenuOperationsCommand { get; set; }
    public RelayCommand HideTextMenuAPICommand { get; set; }

    private SendEventGA4 _sendEventGA4;
    public List<Language> Languages { get; set; }
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
    public RelayCommand LoginCommand { get; set; }
    public RelayCommand ShowSettingsCommand { get; set; }
    public ObservableCollection<ToggleButtons> ToggleButtons { get; set; }
    private List<DispatcherTimer> StopDotTimer { get; set; }
    public RelayCommand TextMenuPinCommand { get; set; }
    public RelayCommand PopupTextMenuCommand { get; set; }
    public RelayCommand TextMenuAPIHeaderActionCommand { get; set; }
    public RelayCommand CopyToClipboardCommand { get; set; }

    public INavigationService NavigationService
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

    public SendEventGA4 SendEventGA4
    {
        get { return _sendEventGA4; }
        set
        {
            _sendEventGA4 = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ButtonViewModel> FixedButtons
    {
        get { return _fixedButtons; }
        set
        {
            _fixedButtons = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<ButtonViewModel> DynamicButtons
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
    
    public bool IsTextEmpty
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

    public double ScrollBarHeight
    {
        get { return _scrollBarHeight; }
        set
        {
            _scrollBarHeight = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ButtonViewModel> TextMenuButtons
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

    public bool IsSpinningJarvisIconTextMenu
    {
        get { return _isSpinningJarvisIconTextMenu; }
        set
        {
            _isSpinningJarvisIconTextMenu = value;
            OnPropertyChanged();
        }
    }

    public string TextMenuAPI
    {
        get { return _textMenuAPI; }
        set
        {
            _textMenuAPI = value;
            OnPropertyChanged();
        }
    }

    public double TextMenuAPIscrollBarHeight
    {
        get { return _textMenuAPIscrollBarHeight; }
        set
        {
            _textMenuAPIscrollBarHeight = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand ShowAIChatBubbleCommand { get; set; }
    public RelayCommand ShowAIChatSidebarCommand { get; set; }
    public RelayCommand AIChatSendCommand { get; set; }
    public RelayCommand NewAIChatCommand { get; set; }
    public RelayCommand HideAIChatSidebarCommand { get; set; }


    private ObservableCollection<AIChatMessage> _aIChatMessages;
    public ObservableCollection<AIChatMessage> AIChatMessages
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

    public bool IsAPIUsageRemain
    {
        get { return _isAPIUsageRemain; }
        set
        {
            _isAPIUsageRemain = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsNoAPIUsageRemain
    {
        get { return _isNoAPIUsageRemain; }
        set
        {
            _isNoAPIUsageRemain = value;
            OnPropertyChanged();
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
    
    public int PreviousCommandIdx
    {
        get { return _previousCommandIdx; }
        set
        {
            _previousCommandIdx = value;
            OnPropertyChanged();
        }
    }

    public static IAutomationElementValueService AutomationElementValueService 
    { 
        get => _automationElementValueService; 
        set => _automationElementValueService = value; 
    }

    public MainViewModel(INavigationService navigationService, 
        PopupDictionaryService popupDictionaryService,
        UIElementDetector accessibilityService, 
        SendEventGA4 sendEventGA4, IAutomationElementValueService automationElementValueService)
    {
        NavigationService = navigationService;
        PopupDictionaryService = popupDictionaryService;
        AccessibilityService = accessibilityService;
        SendEventGA4 = sendEventGA4;
        AutomationElementValueService = automationElementValueService;

        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";

        IsAPIUsageRemain = (RemainingAPIUsage != "0 🔥") ? true : false;

        //TODO:Code test
        if(IsAPIUsageRemain == false)
        {
            WindowLocalStorage.WriteLocalStorage("ApiHeaderID", Guid.NewGuid().ToString());
            WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", "10");
            IsAPIUsageRemain = true;
        }
        IsNoAPIUsageRemain = !IsAPIUsageRemain;

        ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
        HideMenuOperationsCommand = new RelayCommand(o => { PopupDictionaryService.ShowMenuOperations(false); }, o => true);
        
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
        LoginCommand = new RelayCommand(ExecuteLoginCommand, o => true);
        ShowSettingsCommand = new RelayCommand(o => { PopupDictionaryService.IsShowSettingMenu = !PopupDictionaryService.IsShowSettingMenu; }, o => true);
        TextMenuPinCommand = new RelayCommand(ExecuteTextMenuPinCommand, o => true);
        PopupTextMenuCommand = new RelayCommand(ExecutePopupTextMenuCommand, o => true);
        TextMenuAPIHeaderActionCommand = new RelayCommand(ExecuteTextMenuAPIHeaderActionCommand, o => true);
        CopyToClipboardCommand = new RelayCommand(o => { Clipboard.SetText(TextMenuAPI); }, o => true);

        string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = "";
        jsonContent = File.ReadAllText(fullPath);

        Languages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
        TextMenuLanguages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
        LanguageSelectedIndex = 14;

        //Register Acceccibility service
        AccessibilityService.SubscribeToElementFocusChanged();
        EventAggregator.LanguageSelectionChanged += OnLanguageSelectionChanged;
        EventAggregator.AIChatBubbleStatusChanged += OnAIChatBubbleStatusChanged;

        // Checking App update here
        try { ExecuteCheckUpdate(); }

        catch { }
        finally { ExecuteSendEventOpenMainWindow(); }

        InitializeButtons();
        InitializeButtonsTextMenu();

        ShowAIChatBubbleCommand = new RelayCommand(ExecuteShowAIChatBubbleCommand, o => true);
        ShowAIChatSidebarCommand = new RelayCommand(ExecuteShowAIChatSidebarCommand, o => true);
        HideAIChatSidebarCommand = new RelayCommand(ExecuteHideAIChatSidebarCommand, o => true);
        AIChatSendCommand = new RelayCommand(ExecuteAIChatSendCommand, o => true);
        NewAIChatCommand = new RelayCommand(o => { AIChatMessagesClear(); }, o => true);

        AIChatMessages = new ObservableCollection<AIChatMessage>();
        AIChatMessagesClear();
        InitializeSettingToggleButtons();

        Username = "Anh Vu";
        UsernameFirstLetter = Username[0].ToString();

        _globalMouseHook = Hook.GlobalEvents();
        _globalMouseHook.MouseDoubleClick += MouseDoubleClicked;
        _globalMouseHook.MouseDragFinished += MouseDragFinished;
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

    private void OnLanguageSelectionChanged(object sender, EventArgs e)
    {
        AICommand.Execute("Translate it");
    }

    

    private void ExecuteQuitAppCommand(object obj)
    {
        Process.GetCurrentProcess().Kill();
        Task.Run(async () =>
        {
            // Some processing before the await (if needed)
            await Task.Delay(0); // This allows the method to yield to the caller

            await SendEventGA4.SendEvent("quit_app");
        });
    }

    private void ExecuteUndoCommand(object obj)
    {
        try
        {
            AutomationElementValueService.Undo(UIElementDetector.GetInstance().GetFocusingElement());
        }
        catch { }
        finally
        {
        }
    }
    private void ExecuteRedoCommand(object obj)
    {
        try
        {
            AutomationElementValueService.Redo(UIElementDetector.GetInstance().GetFocusingElement());
        }
        catch { }
        finally
        {
        }
    }

    private void ExecuteOpenSettingsCommand(object obj)
    {
        throw new NotImplementedException();
    }

    private void ExecutePinJarvisButtonCommand(object obj)
    {
        PopupDictionaryService.MainWindow.PinJarvisButton();
        PopupDictionaryService.HasPinnedJarvisButton = true;
    }

    //private void GlobalHook_MouseDown(object sender, MouseEventArgs e)
    //{
        //if (e.Button == MouseButtons.Left && _popupDictionaryService.IsShowMenuOperations)
        //{
        //    PresentationSource source = PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow);
        //    Point mousePos = source.CompositionTarget.TransformFromDevice.Transform(new Point(e.X, e.Y));
        //    // Point mousePos = new Point(e.X, e.Y);
        //    Point JarvisMenuPosition = UIElementDetector.PopupDictionaryService.MenuOperationsPosition;

        //    double X1 = JarvisMenuPosition.X;
        //    double Y1 = JarvisMenuPosition.Y;
        //    double X2 = X1 + 400;
        //    double Y2 = Y1 + 165;
        //    if ((mousePos.X < X1 || mousePos.X > X2 || mousePos.Y < Y1 || mousePos.Y > Y2) && (X1 != 0 && Y1 != 0))
        //        HideMenuOperationsCommand.Execute(null);
        //}
    //}

    private async void ExecuteCheckUpdate()
    {
        // Checking App update here
        await SendEventGA4.CheckVersion();
    }

    private async void ExecuteSendEventOpenMainWindow()
    {
        // Starting app
        await SendEventGA4.SendEvent("open_main_window");
    }

    public async void ExecuteShowMenuOperationsCommand(object obj)
    {
        if (!PopupDictionaryService.IsDragging)
        {
            bool _menuShowStatus = PopupDictionaryService.IsShowMenuOperations;
            PopupDictionaryService.ShowMenuOperations(!_menuShowStatus);
            PopupDictionaryService.ShowJarvisAction(false);

            if (_menuShowStatus == false)
            {
                await Task.Run(async () =>
                {
                    // Some processing before the await (if needed)
                    await Task.Delay(0); // This allows the method to yield to the caller

                    await SendEventGA4.SendEvent("open_input_actions");
                });
            }
        }    
    }

    private async void ExecuteExpandCommand(object parameter)
    {
        isExpanded = !isExpanded;
        DynamicButtons[1].Visibility = !isExpanded;

        for (int i = 2; i < DynamicButtons.Count; i++)
            DynamicButtons[i].Visibility = isExpanded;

        UpdateButtonVisibility();
    }

    public async void ExecuteAICommand(object obj)
    {
        string _actionType = (string) obj;
        string _aiAction = "custom";
        try
        {
            bool _fromWindow = false;
            HideMenuOperationsCommand.Execute(null);
            IsSpinningJarvisIcon = true;
            PopupDictionaryService.ShowJarvisAction(true);

            var textFromElement = "";
            var textFromAPI = "";
            try 
            { 
                textFromElement = (String.IsNullOrEmpty(UIElementDetector.CurrentSelectedText))? 
                    AccessibilityService.GetTextFromFocusingEditElement() : 
                    UIElementDetector.CurrentSelectedText;
                Debug.WriteLine($"🎇🎇🎇 TEXT FROM ELEMENT {textFromElement}");
            }
            catch
            {
                textFromElement = MainWindowInputText;
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

            RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";

            IsAPIUsageRemain = (RemainingAPIUsage != "0 🔥") ? true : false;
            IsNoAPIUsageRemain = !IsAPIUsageRemain;


            if (textFromAPI == null)
            {
                Debug.WriteLine($"🆘🆘🆘 {ErrorConstant.translateError}");
                return;
            }
            

            if (_fromWindow != true) { AccessibilityService.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.translateError); }
            else { MainWindowInputText = textFromAPI; }
            AutomationElementValueService.StoreAction(AccessibilityService.GetFocusingElement(), textFromElement);
        }
        catch { }
        finally
        {
            IsSpinningJarvisIcon = false;
            var eventParams = new Dictionary<string, object>
            {
                { "ai_action", _aiAction }
            };

            if (_aiAction == "translate")
                eventParams.Add("ai_action_translate_to", PopupDictionaryService.TargetLangguage);
            else if (_aiAction == "custom")
                eventParams.Add("ai_action_custom", _actionType);

            await SendEventGA4.SendEvent("do_ai_action", eventParams);
        }
    }
    public async void ExecuteHideTextMenuAPICommand(object obj)
    {
        PopupDictionaryService.ShowTextMenuAPIOperations(false);
        PopupDictionaryService.IsShowPinTextMenuAPI = false;
        TextMenuPinColor = "Transparent";
    }
    
    public async void ExecutePopupTextMenuCommand(object obj)
    {
        PopupDictionaryService.IsShowPopupTextMenu = !PopupDictionaryService.IsShowPopupTextMenu;
        if (!PopupDictionaryService.IsShowPinTextMenuAPI) PopupDictionaryService.IsShowTextMenuAPI = false;
    }
    
    private void InitializeButtonsTextMenu()
    {
        AIActionTemplate aIActionTemplate = new AIActionTemplate();
        TextMenuButtons = aIActionTemplate.TextMenuAIActionList;
        TextMenuPinColor = "Transparent";
        int idx = 0;
        foreach (var action in TextMenuButtons)
        {
            action.Command = new RelayCommand(ExecuteTextMenuAICommand, o => true);
            action.PinCommand = new RelayCommand(ExecuteTextMenuPinCommand, o => true);
            action.Idx = idx++;
            action.ExtraIconVisibility = true;
            if(action.PinColor == "") action.PinColor = "Transparent";
        }
    }

    public async void ExecuteTextMenuAICommand(object obj)
    {
        int idx = 0;
        try { idx = (int)obj; }
        catch { idx = int.Parse((string)obj); }
        string _aiAction = "custom";
        string _targetLanguage = TextMenuLanguages[LanguageSelectedIndex].Value;

        PreviousCommandIdx = idx;
        IsActionTranslate = (idx == 0) ? true : false;
        PopupDictionaryService.IsShowPopupTextMenu = IsTextMenuAPIHeaderAction = false;
        try
        {
            TextMenuAPI = "";
            TextMenuAPIscrollBarHeight = 88;
            IsSpinningJarvisIconTextMenu = true;
            TextMenuAPIHeaderActionName = TextMenuButtons[idx].Content;
            PopupDictionaryService.IsShowTextMenuAPI = true;

            var textFromElement = UIElementDetector.CurrentSelectedText;
            var textFromAPI = "";
            
            if (textFromElement == "") return;

            if (TextMenuButtons[idx].CommandParameter == "Translate it")
            {
                textFromAPI = await JarvisApi.Instance.TranslateHandler(textFromElement, _targetLanguage);
                _aiAction = "translate";
            }

            else if (TextMenuButtons[idx].CommandParameter == "Revise it")
            {
                textFromAPI = await JarvisApi.Instance.ReviseHandler(textFromElement);
                _aiAction = "revise";
            }
            else if (TextMenuButtons[idx].CommandParameter == "Ask")
            {
                textFromAPI = await JarvisApi.Instance.AskHandler(textFromElement, FilterText);
                _aiAction = "ask";
            }

            else
                textFromAPI = await JarvisApi.Instance.AIHandler(textFromElement, TextMenuButtons[idx].CommandParameter);

            TextMenuAPI = textFromAPI;

            RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";

            IsAPIUsageRemain = (RemainingAPIUsage != "0 🔥") ? true : false;
            IsNoAPIUsageRemain = !IsAPIUsageRemain;
        }
        catch { }
        finally
        {
            IsSpinningJarvisIconTextMenu = false; // Stop spinning animation
            var eventParams = new Dictionary<string, object>
            {
                { "ai_action", _aiAction }
            };

            if (_aiAction == "translate")
                eventParams.Add("ai_action_translate_to", _targetLanguage);
            else if (_aiAction == "custom")
                eventParams.Add("ai_action_custom", TextMenuButtons[idx].CommandParameter);

            await SendEventGA4.SendEvent("do_ai_action", eventParams);
        }
    }

    private async void ExecuteTextMenuPinCommand(object obj)
    {
        string[] colors = ["Transparent", "#6841EA"];
        int idx = 0;
        try { idx = (int)obj; }
        catch { idx = int.Parse((string)obj); }
        if (idx == -1)
        {
            PopupDictionaryService.IsShowPinTextMenuAPI = !PopupDictionaryService.IsShowPinTextMenuAPI;
            TextMenuPinColor = colors[Convert.ToInt32(PopupDictionaryService.IsShowPinTextMenuAPI)];
            return;
        }

        bool visibilityStatus = !TextMenuButtons[idx].Visibility;
        int sizeChanged = (visibilityStatus) ? -32 : 32;
        TextMenuButtons[idx].Visibility = visibilityStatus;
        
        // Add buttons and expand to the left, the problem is the UI will reload everytime -> not smooth
        //PopupDictionaryService.TextMenuOperationsPosition = new Point
        //(
        //    PopupDictionaryService.TextMenuOperationsPosition.X + sizeChanged, PopupDictionaryService.TextMenuOperationsPosition.Y
        //);

        TextMenuButtons[idx].PinColor = colors[Convert.ToInt32(visibilityStatus)];
        OnPropertyChanged(nameof(TextMenuButtons));
    }
    
    private async void ExecuteTextMenuAPIHeaderActionCommand(object obj)
    {
        IsTextMenuAPIHeaderAction = !IsTextMenuAPIHeaderAction;
        foreach (var action in TextMenuButtons)
        {
            action.ExtraIconVisibility = !IsTextMenuAPIHeaderAction;
        }
    }

    //public async void ExecuteCopyToClipboardCommand(object obj)
    //{
    //    Clipboard.SetText(TextMenuAPI);
    //}

    public async void ExecuteUpgradePlanCommand(object obj)
    {
        try
        {
            string websiteUrl = "https://admin.jarvis.cx/pricing/overview";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = websiteUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            return;
        }
    }

    public async void ExecuteLoginCommand(object obj)
    {
        try
        {
            string websiteUrl = "https://admin.jarvis.cx/auth/login?redirect=%2Fdashboard%2Fsubscription";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = websiteUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            return;
        }
    }

    private void InitializeSettingToggleButtons()
    {
        PopupDictionaryService.JarvisActionVisibility = true;
        PopupDictionaryService.TextMenuSelectionVisibility = true;

        StopDotTimer = new List<DispatcherTimer>();
        ToggleButtons = new ToggleButtonTemplate().ToggleButtonList;

        for (int i = 0; i < ToggleButtons.Count; i++)
        {
            ToggleButtons toggleButton = ToggleButtons[i];

            toggleButton.Idx = i;
            toggleButton.BarBackground = "#1450A3";
            toggleButton.ToggleCommand = new RelayCommand(ExecuteToggleCommand, o => true);
            toggleButton.DotMargin = $"{toggleButton.GridSize.Width - toggleButton.DotSize.Width - 3} 0 0 0";

            DispatcherTimer timer = new DispatcherTimer();            
            timer.Interval = TimeSpan.FromMilliseconds(10); 
            timer.Tick += (sender, e) => Timer_Tick(sender, e, toggleButton.Idx);

            StopDotTimer.Add(timer);
        }
    }
   
    private void ExecuteToggleCommand(object obj)
    {
        int idx = (int)obj;
        if (idx == 0) { PopupDictionaryService.JarvisActionVisibility = !PopupDictionaryService.JarvisActionVisibility; }
        else if (idx == 1) { PopupDictionaryService.TextMenuSelectionVisibility = !PopupDictionaryService.TextMenuSelectionVisibility; }
        
        else if (idx == 2) 
        {

            PopupDictionaryService.IsShowAIChatBubble = (PopupDictionaryService.IsShowAIChatSidebar) ? false : !PopupDictionaryService.IsShowAIChatBubble;
            PopupDictionaryService.IsShowAIChatSidebar = false; 
        }

        ToggleButtons[idx].IsOnline = !ToggleButtons[idx].IsOnline;
        StopDotTimer[idx].Start();
    }

    private void Timer_Tick(object sender, EventArgs e, int idx)
    {
        ToggleButtons toggleButton = ToggleButtons[idx];
        double curMarginX = double.Parse(toggleButton.DotMargin.Split(" ")[0]);
        double minMargin = 3;
        double maxMargin = toggleButton.GridSize.Width - toggleButton.DotSize.Width - minMargin;
        if (toggleButton.IsOnline)
        {
            toggleButton.BarBackground = "#1450A3";
            curMarginX += toggleButton.DotSpeed;
            if (curMarginX >= maxMargin)
            {
                StopDotTimer[idx].Stop();
                curMarginX = maxMargin;
            }
        }
        else
        {
            curMarginX -= toggleButton.DotSpeed;
            toggleButton.BarBackground = "#CCCCCC";
            if (curMarginX <= minMargin)
            {
                StopDotTimer[idx].Stop();
                curMarginX = minMargin;
            }
        }

        toggleButton.DotMargin = $"{curMarginX} 0 0 0";
        OnPropertyChanged(nameof(ToggleButtons));
    }

    public async void ExecuteShowAIChatBubbleCommand(object obj)
    {
        string tmp = (string)obj;
        bool status = (tmp == "false") ? false : true;
        PopupDictionaryService.ShowAIChatBubble(status);
    }

    public async void ExecuteShowAIChatSidebarCommand(object obj)
    {
        PopupDictionaryService.ShowAIChatSidebar(true);
        PopupDictionaryService.ShowAIChatBubble(false);
    }
    
    private async void ExecuteHideAIChatSidebarCommand(object obj)
    {
        PopupDictionaryService.ShowAIChatSidebar(false);
        PopupDictionaryService.ShowAIChatBubble(true);
        AIChatMessagesClear();
    }

    private void OnAIChatBubbleStatusChanged(object sender, EventArgs e)
    {
        ExecuteToggleCommand(2);
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

        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
    }

    private async void MouseDoubleClicked(object sender, System.Windows.Forms.MouseEventArgs e)
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
            PopupDictionaryService.ShowMenuSelectionActions(true);
        }
        else
        {
            System.Windows.Clipboard.SetDataObject(tmpClipboard);
        }
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
            PopupDictionaryService.ShowMenuSelectionActions(true);
        }
        else
        {
            System.Windows.Clipboard.SetDataObject(tmpClipboard);
        }
    }
}


public class AIChatMessage
{
    public bool IsUser { get; set; }
    public bool IsJarvis { get; set; }
    public string Message { get; set; }
    public bool IsLoading { get; set; }
    public bool IsBorderVisible { get; set; }
}

          