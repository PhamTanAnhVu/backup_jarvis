using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions;

public class MenuSelectionResponseViewModel : ViewModelBase
{
    //private PopupDictionaryService? _popupDictionaryService;
    private UIElementDetector? _accessibilityService;
    private SendEventGA4? _googleAnnalyticService;

    private double _scrollBarHeight;
    private int _languageSelectedIndex;
    private bool _isAPIUsageRemain;
    private bool _isNoAPIUsageRemain;
    private bool _isSpinningJarvisIcon;
    private string _remainingAPIUsage;
    private string _selectionTextResponse;
    private string _selectionResponseHeaderName;
    private string _selectionResponsePinColor;
    private AIButton _buttonInfo;
    public RelayCommand TranslateCommand { get; set; }
    public RelayCommand MenuSelectionCommand { get; set; }
    public RelayCommand RedoMenuSelectionCommand { get; set; }
    public RelayCommand MenuSelectionPinCommand { get; set; }
    public RelayCommand ShowMenuSelectionPopupListCommand { get; set; }
    public RelayCommand HideMenuSelectionResponseCommand { get; set; }
    public RelayCommand CopyToClipboardCommand { get; set; }
    public List<Language> TranslateLanguages { get; set; }
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
    public double ScrollBarHeight
    {
        get { return _scrollBarHeight; }
        set
        {
            _scrollBarHeight = value;
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
    
    public string SelectionResponseHeaderName
    {
        get { return _selectionResponseHeaderName; }
        set
        {
            _selectionResponseHeaderName = value;
            OnPropertyChanged();
        }
    }
    public string SelectionTextResponse
    {
        get { return _selectionTextResponse; }
        set
        {
            _selectionTextResponse = value;
            OnPropertyChanged();
        }
    }
    public string SelectionResponsePinColor
    {
        get { return _selectionResponsePinColor; }
        set
        {
            _selectionResponsePinColor = value;
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
    public MenuSelectionResponseViewModel()
    {
        InitializeServices();
        MenuSelectionCommand = new RelayCommand(ExecuteMenuSelectionCommand, o => true);
        MenuSelectionPinCommand = new RelayCommand(ExecuteMenuSelectionPinCommand, o => true);
        ShowMenuSelectionPopupListCommand = new RelayCommand(ExecuteShowMenuSelectionPopupListCommand, o => true);
        HideMenuSelectionResponseCommand = new RelayCommand(ExecuteHideMenuSelectionResponseCommand, o => true);

        TranslateCommand = new RelayCommand(ExecuteTranslateCommand, o => true);
        RedoMenuSelectionCommand = new RelayCommand(o => { MenuSelectionCommand.Execute(_buttonInfo); }, o => true);
        CopyToClipboardCommand = new RelayCommand(o => { Clipboard.SetText(SelectionTextResponse); }, o => true);

        string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = "";
        jsonContent = File.ReadAllText(fullPath);

        TranslateLanguages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
        LanguageSelectedIndex = 14;
        UpdateAPIUsage();

        MenuSelectionSharedData.MenuSelectionCommandExecuted += (sender, e) =>
        {
            MenuSelectionCommand.Execute(sender);
        };
    }

    void InitializeServices()
    {
        _accessibilityService = DependencyInjection.GetService<UIElementDetector>();
        _googleAnnalyticService = DependencyInjection.GetService<SendEventGA4>();
    }

    private void UpdateAPIUsage()
    {
        bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 🔥");
        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
        IsAPIUsageRemain = ((RemainingAPIUsage != "0 🔥") | previousRemaingAPIUSage) ? true : false;
        IsNoAPIUsageRemain = !IsAPIUsageRemain;
    }

    private async void ExecuteHideMenuSelectionResponseCommand(object obj)
    {
        PopupDictionaryService.Instance().IsPinMenuSelectionResponse = false;
        PopupDictionaryService.Instance().IsShowMenuSelectionResponse = false;
        SelectionResponsePinColor = "Transparent";
    }

    private async void ExecuteShowMenuSelectionPopupListCommand(object obj)
    {
        PopupDictionaryService.Instance().IsShowMenuSelectionPopupList = !PopupDictionaryService.Instance().IsShowMenuSelectionPopupList;
        if (PopupDictionaryService.Instance().IsShowMenuSelectionPopupList)
        {
            // Turn off PinCommand for buttons in MenuSelectionResponse View
            MenuSelectionSharedData.PublishMenuSelectionPopupListExecuted(false, EventArgs.Empty);
        }
    }

    public async void ExecuteMenuSelectionPinCommand(object obj)
    {
        string[] colors = { "Transparent", "#6841EA" };
        PopupDictionaryService.Instance().IsPinMenuSelectionResponse = !PopupDictionaryService.Instance().IsPinMenuSelectionResponse;
        SelectionResponsePinColor = colors[Convert.ToInt32(PopupDictionaryService.Instance().IsPinMenuSelectionResponse)];
        if (SelectionResponsePinColor == colors[1])
        {
            await _googleAnnalyticService.SendEvent("pin_inject_selection_actions_response");
        }
    }

    public async void ExecuteTranslateCommand(object obj)
    {
        AIButton buttonInfo = new AIButton
        {
            CommandParameter = "Translate it",
            Content = "Translate"
        };

        MenuSelectionCommand.Execute(buttonInfo);
    }
    public async void ExecuteMenuSelectionCommand(object obj)
    {
        if (!PopupDictionaryService.Instance().IsPinMenuSelectionResponse)
        {
            PopupDictionaryService.Instance().MenuSelectionActionsPosition = new System.Drawing.Point
            (
                PopupDictionaryService.Instance().MenuSelectionActionsPosition.X,
                PopupDictionaryService.Instance().MenuSelectionActionsPosition.Y + 40
            );
        }

        PopupDictionaryService.Instance().IsShowMenuSelectionPopupList = false;

        string _aiAction = "custom";
        string _targetLanguage = TranslateLanguages[LanguageSelectedIndex].Value;
        _buttonInfo = (AIButton)obj;
        
        try
        {
            ScrollBarHeight = 88;
            SelectionTextResponse = "";
            IsSpinningJarvisIcon = true;
            SelectionResponseHeaderName = _buttonInfo.Content;
            PopupDictionaryService.Instance().IsShowMenuSelectionResponse = true;

            var textFromElement = UIElementDetector.CurrentSelectedText;
            if (textFromElement == "") return;

            if (_buttonInfo.CommandParameter == "Translate it")
            {
                SelectionTextResponse = await JarvisApi.Instance.TranslateHandler(textFromElement, _targetLanguage);
                _aiAction = "translate";
            }

            else if (_buttonInfo.CommandParameter == "Revise it")
            {
                SelectionTextResponse = await JarvisApi.Instance.ReviseHandler(textFromElement);
                _aiAction = "revise";
            }

            else
                SelectionTextResponse = await JarvisApi.Instance.AIHandler(textFromElement, _buttonInfo.CommandParameter);
        }
        catch { }
        finally
        {
            UpdateAPIUsage();
            IsSpinningJarvisIcon = false;
            var eventParams = new Dictionary<string, object>
            {
                { "ai_action", _aiAction },
                { "text_selection_action_count", "" }
            };

            if (_aiAction == "translate")
                eventParams.Add("ai_action_translate_to", _targetLanguage);
            else if (_aiAction == "custom")
                eventParams.Add("ai_action_custom", _buttonInfo.CommandParameter);

            await _googleAnnalyticService.SendEvent("do_ai_action", eventParams);
        }
    }
}
