﻿using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
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

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private INavigationService? _navigationService;
    private PopupDictionaryService _popupDictionaryService;
    private UIElementDetector _uIElementDetector;
    private bool _isSpinningJarvisIcon; // Spinning Jarvis icon
    private string _remainingAPIUsage;
    private string _mainWindowInputText;
    private string _filterText;
    private bool _isTextEmpty;
    private bool isExpanded;
    private double _scrollBarHeight;
    private ObservableCollection<ButtonViewModel> _fixedButtons;
    private ObservableCollection<ButtonViewModel> _dynamicButtons;

    private SendEventGA4 _sendEventGA4;
    public List<Language> Languages { get; set; }
    public RelayCommand ShowMenuOperationsCommand { get; set; }
    public RelayCommand HideMenuOperationsCommand { get; set; }
    public RelayCommand AICommand { get; set; }
    public RelayCommand ExpandCommand { get; set; }
    public RelayCommand OpenSettingsCommand { get; set; }
    public RelayCommand QuitAppCommand { get; set; }
    public RelayCommand PinJarvisButtonCommand { get; set; }


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

    public UIElementDetector UIElementDetector
    {
        get { return _uIElementDetector; }
        set
        {
            _uIElementDetector = value;
            OnPropertyChanged();
        }
    }

    // Spinning Jarvis icon
    public bool IsSpinningJarvisIcon
    {
        get { return _isSpinningJarvisIcon; }
        set
        {
            _isSpinningJarvisIcon = value;
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

    public MainViewModel(INavigationService navigationService, PopupDictionaryService popupDictionaryService, UIElementDetector uIElementDetector, SendEventGA4 sendEventGA4)
    {
        NavigationService = navigationService;
        PopupDictionaryService = popupDictionaryService;
        UIElementDetector = uIElementDetector;
        SendEventGA4 = sendEventGA4;

        RemainingAPIUsage = (AppStatus.IsPackaged) 
                                ? $"{Windows.Storage.ApplicationData.Current.LocalSettings.Values["ApiUsageRemaining"]} 🔥"
                                : $"{DataConfiguration.ApiUsageRemaining} 🔥";

        ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
        HideMenuOperationsCommand = new RelayCommand(o => { PopupDictionaryService.ShowMenuOperations(false); }, o => true);
        
        AICommand = new RelayCommand(ExecuteAICommand, o => true);
        ExpandCommand = new RelayCommand(ExecuteExpandCommand, o => true);
        
        OpenSettingsCommand = new RelayCommand(ExecuteOpenSettingsCommand, o => true);
        QuitAppCommand = new RelayCommand(ExecuteQuitAppCommand, o => true);
        PinJarvisButtonCommand = new RelayCommand(ExecutePinJarvisButtonCommand, o => true);

        string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = "";
        jsonContent = File.ReadAllText(fullPath);

        Languages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);

        //Register Acceccibility service
        UIElementDetector.SubscribeToElementFocusChanged();
        EventAggregator.LanguageSelectionChanged += OnLanguageSelectionChanged;
        
        
        // Checking App update here
        try { ExecuteCheckUpdate(); }

        catch { }
        finally { ExecuteSendEventOpenMainWindow(); }

        InitializeButtons();
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
            // Trigger here
            HideMenuOperationsCommand.Execute(null);
            IsSpinningJarvisIcon = true; // Start spinning animation
            PopupDictionaryService.ShowJarvisAction(true);

            var textFromElement = "";
            var textFromAPI = "";
            try { textFromElement = UIElementDetector.GetTextFromFocusingEditElement(); }
            catch
            {
                textFromElement = this.MainWindowInputText;
                _fromWindow = true;
            }

            if (textFromElement == "") return;

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

            if (textFromAPI == null)
            {
                Debug.WriteLine($"🆘🆘🆘 {ErrorConstant.translateError}");
                return;
            }

            RemainingAPIUsage = (AppStatus.IsPackaged)
                                ? $"{Windows.Storage.ApplicationData.Current.LocalSettings.Values["ApiUsageRemaining"]} 🔥"
                                : $"{DataConfiguration.ApiUsageRemaining} 🔥";

            if (_fromWindow != true) { UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.translateError); }
            else { MainWindowInputText = textFromAPI; }
        }
        catch { }
        finally
        {
            IsSpinningJarvisIcon = false; // Stop spinning animation
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
}
