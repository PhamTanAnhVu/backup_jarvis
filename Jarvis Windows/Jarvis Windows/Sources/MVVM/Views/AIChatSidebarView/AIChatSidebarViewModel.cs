//using Jarvis_Windows.Sources.DataAccess.Network;
//using Jarvis_Windows.Sources.Utils.Core;
//using Jarvis_Windows.Sources.Utils.Services;
//using System;
//using System.Collections.ObjectModel;
//using System.Windows.Controls;
//using System.Windows;

using Jarvis_Windows.Sources.Utils.Core;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;
public class AIChatSidebarViewModel : ViewModelBase
{

}

//public class AIChatSidebarViewModel : ViewModelBase
//{
//    private PopupDictionaryService _popupDictionaryService;
//    private GoogleAnalyticService _sendEventGA4;

//    private bool _isTextEmpty;
//    private string _sendMessage;
//    private string _remainingAPIUsage;

//    public RelayCommand SendCommand { get; set; }
//    public RelayCommand NewChatCommand { get; set; }
//    public RelayCommand HideAIChatSidebarCommand { get; set; }

//    private ObservableCollection<AIChatMessage> _aIChatMessages;
//    public ObservableCollection<AIChatMessage> AIChatMessages
//    {
//        get { return _aIChatMessages; }
//        set
//        {
//            _aIChatMessages = value;
//            OnPropertyChanged();
//        }
//    }

//    public PopupDictionaryService PopupDictionaryService
//    {
//        get { return _popupDictionaryService; }
//        set
//        {
//            _popupDictionaryService = value;
//            OnPropertyChanged();
//        }
//    }
//    public GoogleAnalyticService GoogleAnalyticService
//    {
//        get { return _sendEventGA4; }
//        set
//        {
//            _sendEventGA4 = value;
//            OnPropertyChanged();
//        }
//    }
//    public bool IsTextEmpty
//    {
//        get
//        {
//            if (string.IsNullOrWhiteSpace(SendMessage)) _isTextEmpty = true;
//            else _isTextEmpty = false;
//            return _isTextEmpty;
//        }
//        set
//        {
//            _isTextEmpty = value;
//            OnPropertyChanged();
//        }
//    }

//    public string SendMessage
//    {
//        get { return _sendMessage; }
//        set
//        {
//            _sendMessage = value;
//            OnPropertyChanged();
//            OnPropertyChanged(nameof(IsTextEmpty));
//        }
//    }

//    public string RemainingAPIUsage
//    {
//        get { return _remainingAPIUsage; }
//        set
//        {
//            _remainingAPIUsage = value;
//            OnPropertyChanged();
//        }
//    }

//    public AIChatSidebarViewModel(PopupDictionaryService popupDictionaryService, GoogleAnalyticService sendEventGA4)
//    {
//        PopupDictionaryService = popupDictionaryService;
//        GoogleAnalyticService = sendEventGA4;
//        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} ??";

//        HideAIChatSidebarCommand = new RelayCommand(ExecuteHideAIChatSidebarCommand, o => true);
//        SendCommand = new RelayCommand(ExecuteSendCommand, o => true);
//        NewChatCommand = new RelayCommand(o => { ChatMessagesClear(); }, o => true);

//        ChatMessages = new ObservableCollection<ChatMessage>();
//        ChatMessagesClear();
//        EventAggregator.JarvisActionPositionChanged += OnJarvisActionPositionChanged; // Check whether text is in AI ChatSidebar or in external applications
//    }

//    private void OnJarvisActionPositionChanged(object sender, EventArgs e)
//    {
//        string objID = (string)sender;
//    }

//    private async void ExecuteHideAIChatSidebarCommand(object obj)
//    {
//        PopupDictionaryService.ShowAIChatSidebar(false);
//        PopupDictionaryService.ShowAIChatBubble(true);
//        ChatMessagesClear();
//    }

//    void ChatMessagesClear()
//    {
//        AIChatMessages.Clear();
//        AIChatMessages.Add(new AIChatMessage 
//        { 
//            ImageSource = "../../../../Assets/Images/jarvis_logo.png",
//            Message = "Hi, I am Jarvis, your powerful AI assistant. How can I help you?",
//            IsLoading = false,
//            IsBorderVisible = true
//        });
//    }

//    private async void ExecuteSendCommand(object obj)
//    {
//        AIChatMessages.Add(new AIChatMessage
//        {
//            ImageSource = "../../../../Assets/Images/pencil.png",
//            Message = SendMessage,
//            IsLoading = false,
//            IsBorderVisible = false
//        });

//        string tmpMessage = SendMessage;

//        AIChatMessages.Add(new AIChatMessage
//        {
//            ImageSource = "../../../../Assets/Images/jarvis_logo.png",
//            Message = "",
//            IsLoading = true,
//            IsBorderVisible = true
//        });


//        SendMessage = "";

//        int lastIndex = AIChatMessages.Count - 1;
//        string responseMessage = await JarvisApi.Instance.ChatHandler(tmpMessage, AIChatMessages);

//        AIChatMessages.RemoveAt(lastIndex);
//        AIChatMessages.Add(new AIChatMessage
//        {
//            ImageSource = "../../../../Assets/Images/jarvis_logo.png",
//            Message = responseMessage,
//            IsLoading = false,
//            IsBorderVisible = true
//        });

//        RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} ??";
//    }
//}

//public class AIChatMessage
//{
//    public string ImageSource { get; set; }
//    public string Message { get; set; }
//    public bool IsLoading { get; set; }
//    public bool IsBorderVisible { get; set; }
//}
