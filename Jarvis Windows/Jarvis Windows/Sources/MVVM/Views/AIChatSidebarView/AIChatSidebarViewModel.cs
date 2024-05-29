using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Collections.ObjectModel;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Linq;
using Jarvis_Windows.Sources.MVVM.Views.AIRead;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Audio;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using Windows.Globalization;
using System.Threading;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Chat;
using System.Diagnostics;
using System.Security.Cryptography;
using static ScintillaNET.Style;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;
public class AIChatSidebarViewModel : ViewModelBase
{
    private double _dotSpeed;
    private bool _isEmptyAIChatInput;
    private string _aIChatInputMessage;
    private string _aIChatInputSendButtonColor;
    private string _remainingAPIUsage;
    private string _svgToggleIconData;
    private bool _addToolsButtonVisibility;
    private bool _jarvisUpdatedBorderVisibility;
    private bool _isShowChatHistory;
    private bool _isShowIntro;
    private bool _isShowChatConversation;
    private bool _isOpenSelectAIModel;
    private bool _isProcessAIChat;
    private bool _isOutOfToken;
    private bool _isLoadingConversation;

    private GoogleAnalyticService _googleAnalyticService;

    public string AddToolsButtonBorder;
    public RelayCommand OpenJarvisWebsiteCommand { get; set; }
    public RelayCommand ToggleAddToolsCommand { get; set; }
    public RelayCommand SendChatInputCommand { get; set; }
    public RelayCommand NewAIChatWindowCommand { get; set; }
    public RelayCommand CloseJarvisUpdatedCommand { get; set; }
    public RelayCommand ShowChatHistory { get; set; }
    public RelayCommand OpenSelectAIModelCommand { get; set; }
    public RelayCommand CloseOutOfTokenPopupCommand { get; set; }
    public RelayCommand SelectModel { get; set; }
    public GoogleAnalyticService GoogleAnalyticService
    {
        get { return _googleAnalyticService; }
        set
        {
            _googleAnalyticService = value;
            OnPropertyChanged();
        }
    }
    public bool IsEmptyAIChatInput
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AIChatInputMessage)) _isEmptyAIChatInput = true;
            else _isEmptyAIChatInput = false;
            return _isEmptyAIChatInput;
        }
        set
        {
            _isEmptyAIChatInput = value;
            OnPropertyChanged();
        }
    }

    public string AIChatInputMessage
    {
        get { return _aIChatInputMessage; }
        set
        {
            _aIChatInputMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyAIChatInput));
            OnPropertyChanged(nameof(AIChatInputSendButtonColor));
        }
    }

    public string AIChatInputSendButtonColor
    {
        get 
        {
            if (IsEmptyAIChatInput) _aIChatInputSendButtonColor = "#CBD5E1";
            else _aIChatInputSendButtonColor = "#0078D4";
            return _aIChatInputSendButtonColor; 
        }
        set
        {
            _aIChatInputSendButtonColor = value;
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
    public string SvgToggleIconData
    {
        get { return _svgToggleIconData; }
        set
        {
            _svgToggleIconData = value;
            OnPropertyChanged();
        }
    }
    public bool AddToolsButtonVisibility
    {
        get { return _addToolsButtonVisibility; }
        set
        {
            _addToolsButtonVisibility = value;
            OnPropertyChanged();
        }
    }
    public bool JarvisUpdatedBorderVisibility
    {
        get { return _jarvisUpdatedBorderVisibility; }
        set
        {
            _jarvisUpdatedBorderVisibility = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowChatHistory
    {
        get { return _isShowChatHistory; }
        set
        {
            _isShowChatHistory = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowIntro
    {
        get { return _isShowIntro; }
        set
        {
            _isShowIntro = value;
            IsShowChatConversation = !_isShowIntro;
            OnPropertyChanged();
        }
    }
    public bool IsShowChatConversation
    {
        get { return _isShowChatConversation; }
        set
        {
            _isShowChatConversation = value;
            OnPropertyChanged();
        }
    }
    public bool IsOpenSelectAIModel
    {
        get { return _isOpenSelectAIModel; }
        set
        {
            _isOpenSelectAIModel = value;
            OnPropertyChanged();
        }
    }
    public bool IsOutOfToken
    {
        get { return _isOutOfToken; }
        set
        {
            _isOutOfToken = value;
            OnPropertyChanged();
        }
    }
    public bool IsLoadingConversation
    {
        get { return _isLoadingConversation; }
        set
        {
            _isLoadingConversation = value;
            OnPropertyChanged();
        }
    }

    private List<DispatcherTimer> StopDotTimer { get; set; }
    public ObservableCollection<AddToolsToggleButton> ToggleButtons { get; set; }
    public ObservableCollection<AddToolsToggleButton> BasicToggleButtons { get; set; }
    public ObservableCollection<AddToolsToggleButton> AdvancedToggleButtons { get; set; }
    public ChatHistoryViewModel ChatHistoryViewModel { get; set; }

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

    public AIChatSidebarViewModel()
    {
        try
        {
            Task.Run(async () => await ResetAPIUsageDaily()).Wait();
        }
        catch { }

        OpenJarvisWebsiteCommand = new RelayCommand(ExecuteOpenJarvisWebsiteCommand, o => true);
        ToggleAddToolsCommand = new RelayCommand(ExecuteToggleAddToolsCommand, o => true);
        SendChatInputCommand = new RelayCommand(ExecuteSendChatInputCommand, o => true);
        CloseJarvisUpdatedCommand = new RelayCommand(ExecuteCloseJarvisUpdatedCommand, o => true);
        NewAIChatWindowCommand = new RelayCommand(ExecuteNewAIChatWindowCommand, o => true);
        OpenSelectAIModelCommand = new RelayCommand(o => { IsOpenSelectAIModel = !IsOpenSelectAIModel; }, o => true);
        CloseOutOfTokenPopupCommand = new RelayCommand(o => { IsOutOfToken = false; }, o => true);
        SelectModel = new RelayCommand(o => { IsOpenSelectAIModel = false; }, o => true);
        ShowChatHistory = new RelayCommand(o => { IsShowChatHistory = !IsShowChatHistory; ChatHistoryViewModel.UpdateLastUpdatedTime(); }, o => true);
        JarvisUpdatedBorderVisibility = false; 
        
        AddToolsButtonVisibility = true;
        ToggleAddToolsCommand.Execute(null);
        InitializeToggleButtons();

        EventSubscribe();

        ChatHistoryViewModel = new ChatHistoryViewModel();
        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")}";
    }

    private void EventSubscribe()
    {
        AIChatSidebarEventTrigger.SelectConversationChanged += OnSelectConversation;

        AIChatSidebarEventTrigger.MouseOverInfoPopup += (sender, e) =>
        {
            ExecuteInfoPopupCommand(-1);
        };

        EventAggregator.ApiUsageChanged += (sender, e) =>
        {
            RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")}";
        };
    }

    private async Task ResetAPIUsageDaily()
    {
        await JarvisApi.Instance.APIUsageHandler();
    }

    // Extra bullshit
    private async void ExecuteOpenJarvisWebsiteCommand(object obj)
    {
        string websiteUrl = (string)obj;
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = websiteUrl,
            UseShellExecute = true
        });
    }
    
    private async void ExecuteCloseJarvisUpdatedCommand(object obj)
    {
        JarvisUpdatedBorderVisibility = false;
    }

    private async void ExecuteInfoPopupCommand(object obj)
    {
        int idx = (int)obj;
        for (int i = 0; i < ToggleButtons.Count; i++)
        {
            if (i == idx) continue;
            ToggleButtons[i].IsOpenInfoPopup = false;
        }

        if (idx == -1) return;
        ToggleButtons[idx].IsOpenInfoPopup = !ToggleButtons[idx].IsOpenInfoPopup;
    }

    private void InitializeToggleButtons()
    {
        _dotSpeed = 2;

        ToggleButtons = new ObservableCollection<AddToolsToggleButton>();
        StopDotTimer = new List<DispatcherTimer>();
        
        string[] headers = { "Web Access", "Create Images (DALL·E 3)", "Book Calendar Events", "Advanced Data Analysis" };
        string[] popupDescription = 
        { 
            "When needed, Jarvis can search the internet or\n read the URLs you provide to obtain the real-time information and reduce hallucinations.",
            "Jarvis can use DALL·E 3 to create images for you based on your needs and continue to improve it according to your requirements.",
            "Jarvis can create calendar events from your request or from a flight screenshot. The event will be sent to you via email. You can then add the event to your favorite calendar app from the email.",
            "Jarvis can use DALL·E 3 to create images for you based on your needs and continue to improve it according to your requirements."
        };

        string[] popupButtonName = { "Search for latest AI news", "Draw a cute dog", "Create a schedule for my family part", "Generate a revenue report" };
        for (int idx = 0; idx < headers.Length; idx++)
        {
            AddToolsToggleButton toggleButton = new AddToolsToggleButton
            {
                Idx = idx,
                IsActive = false,
                ToggleButtonWidth = 28,
                ToggleButtonHeight = 16,
                ToggleDotSize = 14,
                ToggleDotMargin = "1 0 0 0",
                ToggleBackground = "#CBD5E1",
                ToggleCommand = new RelayCommand(ExecuteToggleCommand, o => true),
                InfoPopupCommand = new RelayCommand(ExecuteInfoPopupCommand, o => true),

                IsOpenInfoPopup = false,
                Header = headers[idx],
                InfoPopupDescription = popupDescription[idx],
                InfoPopupButtonName = popupButtonName[idx],
            };

            bool isActive = false;
            if (isActive)
            {
                toggleButton.IsActive = true;
                toggleButton.ToggleBackground = "#0078D4";
                toggleButton.ToggleDotMargin = $"{toggleButton.ToggleButtonWidth - toggleButton.ToggleDotSize - 1} 0 0 0";
            }
            ToggleButtons.Add(toggleButton);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (sender, e) => Timer_Tick(sender, e, toggleButton.Idx);
            StopDotTimer.Add(timer);
        }


        for (int idx = 0; idx < ToggleButtons.Count; idx++)
        {
            ToggleButtons[idx].SvgData = SvgDataTemplate.Instance().AIChatSvgDataTemplate[idx];
        }

        BasicToggleButtons = new ObservableCollection<AddToolsToggleButton>(ToggleButtons.Take(1));
        AdvancedToggleButtons = new ObservableCollection<AddToolsToggleButton>(ToggleButtons.Skip(1));
    }

    private async void ExecuteToggleCommand(object obj)
    {
        int idx = (int)obj;
        ToggleButtons[idx].IsActive = !ToggleButtons[idx].IsActive;
        StopDotTimer[idx].Start();

        if (idx == 1)
        {

        }
        else if (idx == 2)
        {

        }

        else if (idx == 3)
        {

        }
        else if (idx == 4)
        {

        }
        //SettingStatus[idx] = (ToggleButtons[idx].IsActive) ? '1' : '0';
        //WindowLocalStorage.WriteLocalStorage("SettingStatus", SettingStatus.ToString());
    }
    private void Timer_Tick(object sender, EventArgs e, int idx)
    {
        AddToolsToggleButton toggleButton = ToggleButtons[idx];
        double curMarginX = double.Parse(toggleButton.ToggleDotMargin.Split(" ")[0]);
        double minMargin = 1;
        double maxMargin = toggleButton.ToggleButtonWidth - toggleButton.ToggleDotSize - minMargin;
        if (toggleButton.IsActive)
        {
            toggleButton.ToggleBackground = "#0078D4";
            curMarginX += _dotSpeed;
            if (curMarginX >= maxMargin)
            {
                StopDotTimer[idx].Stop();
                curMarginX = maxMargin;
            }
        }
        else
        {
            curMarginX -= _dotSpeed;
            toggleButton.ToggleBackground = "#CBD5E1";
            if (curMarginX <= minMargin)
            {
                StopDotTimer[idx].Stop();
                curMarginX = minMargin;
            }
        }

        toggleButton.ToggleDotMargin = $"{curMarginX} 0 0 0";
    }

    private async void ExecuteToggleAddToolsCommand(object obj)
    {
        string[] borderRadius = { "12", "0 12 12 0" };
        string[] svgData = 
        {
            "M1.44064 4.06564C1.6115 3.89479 1.8885 3.89479 2.05936 4.06564L7 9.00628L11.9406 4.06564C12.1115 3.89479 12.3885 3.89479 12.5594 4.06564C12.7302 4.2365 12.7302 4.5135 12.5594 4.68436L7.30936 9.93436C7.1385 10.1052 6.8615 10.1052 6.69064 9.93436L1.44064 4.68436C1.26979 4.5135 1.26979 4.2365 1.44064 4.06564Z",
            "M12.5594 9.93436C12.3885 10.1052 12.1115 10.1052 11.9406 9.93436L7 4.99372L2.05936 9.93436C1.8885 10.1052 1.6115 10.1052 1.44064 9.93436C1.26979 9.7635 1.26979 9.4865 1.44064 9.31564L6.69064 4.06564C6.8615 3.89479 7.1385 3.89479 7.30936 4.06564L12.5594 9.31564C12.7302 9.4865 12.7302 9.7635 12.5594 9.93436Z"
        };

        AddToolsButtonVisibility = !AddToolsButtonVisibility;
        int idx = (AddToolsButtonVisibility) ? 1 : 0;
        
        AddToolsButtonBorder = borderRadius[idx];
        SvgToggleIconData = svgData[idx];
    }


    // ChatMessage
    private async void OnSelectConversation(object obj, EventArgs e)
    {
        if (_isProcessAIChat || IsLoadingConversation) return;

        int idx = (int)obj;

        if (idx != -1)
        {
            IsShowChatHistory = false;
            if (ConversationManager.Instance()._selectedIdx != idx)
            {
                ChatHistoryViewModel.DeselectConversation();
                ConversationManager.Instance()._selectedIdx = idx;
                UpdateConversation(idx);

            }
            else if (AIChatMessages is not null) { return; }
            await LoadChatMessagesAsync();

            AIChatSidebarEventTrigger.PublishScrollChatToBottom(true, EventArgs.Empty);

        }

        else if (ConversationManager.Instance()._selectedIdx == -1)
        {
            IsShowIntro = true;
            AIChatMessages = new ObservableCollection<AIChatMessage>();
        }
    }

    private async Task RenderBatchOfMessages(int L, int R)
    {
        for (int i = L; i < R; i++)
        {
            string message = AIChatMessages[i].Message;
            int messageIdx = AIChatMessages[i].Idx;
            bool isUser = AIChatMessages[i].IsUser;
            AIChatMessages[i] = CreateChatMessage(messageIdx, message, isUser, false);
        }
    }

    private async Task LoadBatch()
    {
        int batchSize = 5;
        for (int i = AIChatMessages.Count; i >= 0; i -= batchSize)
        {
            int L = (i - batchSize < 0) ? 0 : i - batchSize;
            int R = i;
            await RenderBatchOfMessages(L, R);
            await Task.Delay(500);
        }   
    }

    private void UpdateConversation(int idx)
    {
        ChatHistoryViewModel.DeselectConversation();
        ChatHistoryViewModel.ConversationList[idx].IsSelected = true;
        ConversationManager.Instance().UpdateConversation(ChatHistoryViewModel.ConversationList[idx]);
    }

    private async Task LoadChatMessagesAsync()
    {
        IsShowIntro = false;
        IsLoadingConversation = true;
        await Task.Delay(500);

        var messages = await Task.Run(() =>
            ConversationManager.Instance().LoadChatMessages(ConversationManager.Instance()._selectedIdx)
        );

        AIChatMessages = messages;
        LoadBatch();
        await Task.Delay(1500);
        IsLoadingConversation = false;
    }

    /*======================== For testing ========================*/
    private void DeleteMessages()
    {
        if (AIChatMessages is null) return;
        for (int i = 0; i < 30; i++)
        {
            AIChatMessages.RemoveAt(AIChatMessages.Count - 1);
        }
    }

    private void AddMoreMessages()
    {
        int curConversation = ConversationManager.Instance()._selectedIdx;
        for (int i = 0; i < 15; i++)
        {
            int n = AIChatMessages.Count - 1;
            AIChatMessage lastUser = AIChatMessages[n - 1];
            AIChatMessage lastServer = AIChatMessages[n];
            AIChatMessages.Add(lastUser);
            ConversationManager.Instance().UpdateChatMessage(lastUser, false, curConversation);
            AIChatMessages.Add(lastServer);
            ConversationManager.Instance().UpdateChatMessage(lastServer, false, curConversation);
        }
    }

    /* ========================================== */

    private AIChatMessage CreateChatMessage(int idx, string message, bool isUser, bool isLoading = false)
    {
        return new AIChatMessage
        {
            IsUser = isUser,
            IsServer = !isUser,
            IsLoading = isLoading,
            Message = message,
            Idx = idx,
            CopyCommand = new RelayCommand(ExecuteCopyCommand, o => true),
            RedoCommand = new RelayCommand(ExecuteRedoCommand, o => true),
            DetailMessage = isUser ? new ObservableCollection<CodeMessage> { new CodeMessage { TextContent = message, IsVisible = false } }
                            : RetrieveCodeSection(message)
        };
    }

    private async void ExecuteCopyCommand(object obj)
    {
        int idx = (int)obj;
        try 
        {
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetDataObject(AIChatMessages[idx].Message);
        }
        catch { return;}
    }

    private async void ExecuteNewAIChatWindowCommand(object obj)
    {
        if (_isProcessAIChat) { return; }

        IsLoadingConversation = false;
        ChatHistoryViewModel.DeselectConversation();
        ConversationManager.Instance()._selectedIdx = -1;
        AIChatMessages = ConversationManager.Instance().LoadChatMessages(ConversationManager.Instance()._selectedIdx);
        IsShowIntro = true;
    }

    
    private async void ExecuteSendChatInputCommand(object obj)
    {
        // If server is processing request/empty input message then cancel execution
        if (_isProcessAIChat || IsLoadingConversation) return;
        if (string.IsNullOrEmpty(AIChatInputMessage)) return;
        
        int curConversation = ConversationManager.Instance()._selectedIdx;

        // User runs out of token, stop
        if (RemainingAPIUsage == "0")
        {
            _isProcessAIChat = false;
            IsOutOfToken = true;
            return;
        }
        
        _isProcessAIChat = true;
        IsShowIntro = false;

        

        // obj null = add new message to the end, else = update
        int index = (obj is null) ? AIChatMessages.Count : (int)obj;
        bool isUpdated = (obj is not null);

        // User send message, update AIChatMessages and DB
        var sendMessage = CreateChatMessage(index, AIChatInputMessage, true);
        AIChatMessages.Insert(index, sendMessage);
        if (!isUpdated) { AIChatSidebarEventTrigger.PublishScrollChatToBottom(true, EventArgs.Empty); }
        await ConversationManager.Instance().UpdateChatMessage(sendMessage, isUpdated, curConversation);

        // Clear InputMessage after sending, save it to tmp
        string inputChatMessage = AIChatInputMessage;
        AIChatInputMessage = "";

        // Server is responding, showing loading icons
        var responseMessage = CreateChatMessage(index + 1, "", false, true);
        AIChatMessages.Insert(index + 1, responseMessage);

        // Server returns result, update AIChatMessages and DB
        string? textResponse = await JarvisApi.Instance.ChatHandler(inputChatMessage, AIChatMessages);
        textResponse = (string.IsNullOrEmpty(textResponse)) ? "**There is something wrong. Please try again later.**" : textResponse;
        responseMessage = CreateChatMessage(index + 1, textResponse, false);
        AIChatMessages.RemoveAt(index + 1);
        AIChatMessages.Insert(index + 1, responseMessage);
        

        await ConversationManager.Instance().UpdateChatMessage(responseMessage, isUpdated, curConversation);

        // Server finish processing, update APIUsage
        _isProcessAIChat = false;
        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")}";
        if (!isUpdated) { AIChatSidebarEventTrigger.PublishScrollChatToBottom(true, EventArgs.Empty); }
        ChatHistoryViewModel.InitConversation(ConversationManager.Instance()._selectedIdx);
    }

    private async void ExecuteRedoCommand(object obj)
    {
        if (_isProcessAIChat) return;
        int idx = (int)obj;
        AIChatInputMessage = AIChatMessages[idx - 1].Message;
        idx -= 1;
        AIChatMessages.RemoveAt(idx);
        AIChatMessages.RemoveAt(idx);
        SendChatInputCommand.Execute(idx);
    }

    private static ObservableCollection<CodeMessage> RetrieveCodeSection(string inputText)
    {
        ObservableCollection<CodeMessage> sections = new ObservableCollection<CodeMessage>();
        var codeSectionMatches = Regex.Matches(inputText, @"```(?<Language>\w+)\s*(?<content>[\s\S]+?)```");

        string[] textSelection = Regex.Split(inputText, @"```(?:\w+)\s*[\s\S]+?```");

        foreach (Match match in codeSectionMatches)
        {
            string Language = match.Groups["Language"].Value;
            if (Language == "csharp") { Language = "c#"; }
            if (Language == "cpp" || Language == "c") { Language = "c++"; }
            if (Language == "javascript") { Language = "JavaScript"; }
            if (Language == "html" || Language == "css" || Language == "xml" || Language == "php")
            {
                Language = Language.ToUpper();
            }

            if (!string.IsNullOrEmpty(Language))
            {
                Language = char.ToUpper(Language[0]) + Language.Substring(1);
            }
            string content = match.Groups["content"].Value.Trim('\n');
            sections.Add(new CodeMessage { Language = Language, CodeContent = content, IsVisible = true });
        }

        for (int idx = 0; idx < textSelection.Length; idx++)
        {
            string textContent = textSelection[idx].TrimStart('\n').TrimEnd('\n');

            if (textContent.Length > 1)
            {
                if (idx > 0) { textContent = "\n" + textContent; }
                if (idx < textSelection.Length - 1) { textContent += "\n"; }
            }

            if (idx < sections.Count)
            {
                sections[idx].TextContent = textContent;
            }
            else
            {
                sections.Add(new CodeMessage { TextContent = textContent, IsVisible = false });
            }


            sections[idx].Idx = idx;
        }

        return sections;
    }
}