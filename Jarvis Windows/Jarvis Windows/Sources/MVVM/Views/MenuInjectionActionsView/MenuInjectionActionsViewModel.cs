using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.DataAccess;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.MVVM.Views.InjectionAction;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

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
        public RelayCommand UndoCommand { get; set; }
        public RelayCommand RedoCommand { get; set; }
        public RelayCommand UpgradePlanCommand { get; set; }
        public RelayCommand? CopyToClipboardCommand { get; set; }
        public RelayCommand? CloseOutOfTokenPopupCommand { get; set; }

        public string? RemainingAPIUsage
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
            //    WindowLocalStorage.WriteLocalStorage("ApiHeaderID", Guid.NewGuid().ToString());
            //    WindowLocalStorage.WriteLocalStorage("ApiUsageRemaining", "10");
            //    RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
            //    IsAPIUsageRemain = true;
            //}

            ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
            HideMenuOperationsCommand = new RelayCommand(ExecuteHideMenuOperationsCommand, o => true);

            AICommand = new RelayCommand(ExecuteAICommand, o => true);
            ExpandCommand = new RelayCommand(ExecuteExpandCommand, o => true);

            UndoCommand = new RelayCommand(ExecuteUndoCommand, o => true);
            RedoCommand = new RelayCommand(ExecuteRedoCommand, o => true);
            UpgradePlanCommand = new RelayCommand(ExecuteUpgradePlanCommand, o => true);
            CloseOutOfTokenPopupCommand = new RelayCommand(o => { IsOutOfToken = false; }, o => true);

            string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
            string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
            string jsonContent = "";
            jsonContent = File.ReadAllText(fullPath);

            Languages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
            LanguageSelectedIndex = 14;
            _authUrl = DataConfiguration.AuthUrl;
            EventAggregator.LanguageSelectionChanged += OnLanguageSelectionChanged;

            try { ExecuteCheckUpdate(); }

            catch { }
            finally { ExecuteSendEventOpenMainWindow(); }

            try { ExecuteGetUserGeoLocation(); }
            catch { }

            InitializeButtons();

            EventAggregator.ApiUsageChanged += (sender, e) =>
            {
                RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
                IsOutOfToken = (RemainingAPIUsage == "0 🔥") ? true : false;
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
            AICommand.Execute("Translate it");
        }

        private void ExecuteHideMenuOperationsCommand(object obj)
        {
            PopupDictionaryService.Instance().IsShowMenuOperations = false;
            PopupDictionaryService.Instance().ShowJarvisAction(true);
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
        }
        public void ExecuteUpgradePlanCommand(object obj)
        {
            string websiteUrl = _authUrl;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = websiteUrl,
                UseShellExecute = true
            });
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (!string.IsNullOrEmpty(propertyName) && propertyName.Equals("TokenService"))
            {
                bool previousRemaingAPIUSage = (RemainingAPIUsage != "0 🔥");
                RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} 🔥";
                IsOutOfToken = ((RemainingAPIUsage == "0 🔥") | previousRemaingAPIUSage) ? true : false;
            }
        }
    }
}
