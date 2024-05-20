using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public class ChatHistoryViewModel : ViewModelBase
{
    private double _opacity;
    private int _conversationIdx;
    private bool _isTitleEditable;
    private bool _isOpenDeletePopup;
    private string _title;
    private string _deleteDescription;
    private string _filterFavoriteColor;
    private string _aIChatHistorySearchMessage;
    private bool _isEmptyAIChatHistorySearchInput;
    private bool _isHitTestVisible;
    private bool _isShowPopup;
    private bool _isNotEmptyChatHistory;
    private bool _isEmptyChatHistory;

    private string[] _favoriteColors;
    private string[] _favoriteDatas;
    public RelayCommand AIChatHistorySearchSendCommand { get; set; }
    public RelayCommand FilterFavoriteChatCommand { get; set; }
    public RelayCommand CloseEditTitleCommand { get; set; }
    public RelayCommand OpenDeletePopupCommand { get; set; }
    public RelayCommand CloseDeletePopupCommand { get; set; }
    public RelayCommand DeleteCommand { get; set; }
    public RelayCommand SaveEditTitleCommand { get; set; }

    private ObservableCollection<ConversationModel> _conversationList;
    public ObservableCollection<ConversationModel> ConversationList
    {
        get { return _conversationList; }
        set
        {
            _conversationList = value;
            OnPropertyChanged();
        }
    }

    public double Opacity
    {
        get { return _opacity; }
        set
        {
            _opacity = value;
            OnPropertyChanged();
        }
    }

    public int ConversationIdx
    {
        get { return _conversationIdx; }
        set
        {
            _conversationIdx = value;
            OnPropertyChanged();
        }
    }


    public bool IsTitleEditable
    {
        get { return _isTitleEditable; }
        set
        {
            _isTitleEditable = value;
            OnPropertyChanged();
            IsHitTestVisible = !_isTitleEditable;
            ConversationIdx = (!_isTitleEditable) ? -1 : ConversationIdx;
            IsShowPopup = IsTitleEditable | IsOpenDeletePopup;
        }
    }

    public bool IsOpenDeletePopup
    {
        get { return _isOpenDeletePopup; }
        set
        {
            _isOpenDeletePopup = value;
            OnPropertyChanged();
            IsHitTestVisible = !_isOpenDeletePopup;
            ConversationIdx = (!_isOpenDeletePopup) ? -1 : ConversationIdx;
            IsShowPopup = IsTitleEditable | IsOpenDeletePopup;
        }
    }

    public bool IsHitTestVisible
    {
        get { return _isHitTestVisible; }
        set
        {
            _isHitTestVisible = value;
            OnPropertyChanged();
            Opacity = (_isHitTestVisible) ? 1 : 0.7;
        }
    }

    public bool IsShowPopup
    {
        get { return _isShowPopup; }
        set
        {
            _isShowPopup = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }
    public string DeleteDescription
    {
        get { return _deleteDescription; }
        set
        {
            _deleteDescription = value;
            OnPropertyChanged();
        }
    }

    public string FilterFavoriteColor
    {
        get { return _filterFavoriteColor; }
        set
        {
            if (value == "change")
            {
                value = (_filterFavoriteColor == _favoriteColors[0]) ? _favoriteColors[1] : _favoriteColors[0];
            }

            _filterFavoriteColor = value;
            OnPropertyChanged();
        }
    }

    public bool IsEmptyAIChatHistorySearchInput
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AIChatHistorySearchMessage)) _isEmptyAIChatHistorySearchInput = true;
            else _isEmptyAIChatHistorySearchInput = false;
            return _isEmptyAIChatHistorySearchInput;
        }
        set
        {
            _isEmptyAIChatHistorySearchInput = value;
            OnPropertyChanged();
        }
    }
    public bool IsNotEmptyChatHistory
    {
        get { return _isNotEmptyChatHistory; }
        set
        {
            _isNotEmptyChatHistory = value;
            OnPropertyChanged();
        }
    }
    public bool IsEmptyChatHistory
    {
        get { return _isEmptyChatHistory; }
        set
        {
            _isEmptyChatHistory = value;
            OnPropertyChanged();
        }
    }

    public string AIChatHistorySearchMessage
    {
        get { return _aIChatHistorySearchMessage; }
        set
        {
            _aIChatHistorySearchMessage = value;
            OnPropertyChanged();
            FilterConversation();
            OnPropertyChanged(nameof(IsEmptyAIChatHistorySearchInput));
        }
    }

    public ChatHistoryViewModel()
    {
        InitConversationList();
        CloseEditTitleCommand = new RelayCommand(o => { IsTitleEditable = false; }, o => true);
        OpenDeletePopupCommand = new RelayCommand(ExecuteOpenDeletePopupCommand, o => true);
        CloseDeletePopupCommand = new RelayCommand(o => { IsOpenDeletePopup = false; }, o => true);
        DeleteCommand = new RelayCommand(ExecuteDeleteCommand, o => true);
        SaveEditTitleCommand = new RelayCommand(ExecuteSaveEditTitleCommand, o => true);
        FilterFavoriteChatCommand = new RelayCommand(ExecuteFilterFavoriteChatCommand, o => true);

        IsNotEmptyChatHistory = (ConversationList.Count != 0);
        IsEmptyChatHistory = (ConversationList.Count == 0);
    }

    private void InitConversationList()
    {
        _favoriteColors = ["#64748B", "#FACC15"];
        _favoriteDatas =
        [
            "M2.14959 11.1381C2.09124 11.4707 2.41941 11.731 2.70924 11.582L6.00116 9.89008L9.29308 11.582C9.58291 11.731 9.91108 11.4707 9.85273 11.1381L9.23044 7.59095L11.8722 5.07347C12.1191 4.83819 11.9913 4.40802 11.6605 4.36104L7.98667 3.83914L6.34858 0.594229C6.20102 0.301924 5.8013 0.301924 5.65374 0.594229L4.01565 3.83914L0.341789 4.36104C0.0110467 4.40802 -0.116753 4.83819 0.130132 5.07347L2.77188 7.59095L2.14959 11.1381ZM5.82804 9.06196L3.06372 10.4827L3.58442 7.51478C3.60896 7.37487 3.56281 7.2316 3.46236 7.13588L1.28223 5.0583L4.32146 4.62655C4.44709 4.6087 4.5568 4.52846 4.61606 4.41106L6.00116 1.66731L7.38626 4.41106C7.44552 4.52846 7.55523 4.6087 7.68086 4.62655L10.7201 5.0583L8.53996 7.13588C8.43951 7.2316 8.39336 7.37487 8.4179 7.51478L8.93859 10.4827L6.17428 9.06196C6.06509 9.00584 5.93723 9.00584 5.82804 9.06196Z",
            "M2.70924 11.582C2.41941 11.731 2.09124 11.4707 2.14959 11.1381L2.77188 7.59095L0.130132 5.07347C-0.116753 4.83819 0.0110467 4.40802 0.341789 4.36104L4.01565 3.83914L5.65374 0.594229C5.8013 0.301924 6.20102 0.301924 6.34858 0.594229L7.98667 3.83914L11.6605 4.36104C11.9913 4.40802 12.1191 4.83819 11.8722 5.07347L9.23044 7.59095L9.85273 11.1381C9.91108 11.4707 9.58291 11.731 9.29308 11.582L6.00116 9.89008L2.70924 11.582Z",
        ];

        FilterFavoriteColor = _favoriteColors[0];
        ConversationList = new ObservableCollection<ConversationModel>
        {
            new ConversationModel
            {
                Title = "1. New Chat",
                LastMessage = "No thanks for you stupid fucking resume. Fortunately, you have actually uploaded or shared your resume document. Without the actual resume file, I am unable to review and provide feedback on its content, formatting, and structure.",

                LastUpdatedTime = "5 hours ago",
                IsSelected = true,
                IsShowConversation = true,
                FavoriteData = _favoriteDatas[0],
                FavoriteColor = _favoriteColors[0],
                SelectConversationCommand = new RelayCommand(ExecuteSelectConversationCommand, o => true),
                EditTitleCommand = new RelayCommand(ExecuteEditTitleCommand, o => true),
                MarkFavoriteCommand = new RelayCommand(ExecuteMarkFavoriteCommand, o => true),
                OpenDeletePopupCommand = new RelayCommand(ExecuteOpenDeletePopupCommand, o => true),
            },

            new ConversationModel
            {
                Title = "2. T1 vs TL MSI 2024 - Bracket Stage",
                LastMessage = "I am unable to review and provide feedback on its content, formatting In Hadoop, Java is commo",
                LastUpdatedTime = "5 hours ago",
                IsSelected = false,
                IsShowConversation = true,
                FavoriteData = _favoriteDatas[0],
                FavoriteColor = _favoriteColors[0],
                SelectConversationCommand = new RelayCommand(ExecuteSelectConversationCommand, o => true),
                EditTitleCommand = new RelayCommand(ExecuteEditTitleCommand, o => true),
                MarkFavoriteCommand = new RelayCommand(ExecuteMarkFavoriteCommand, o => true),
                OpenDeletePopupCommand = new RelayCommand(ExecuteOpenDeletePopupCommand, o => true),
            },

            new ConversationModel
            {
                Title = "3. Gemini What are you doing bruh ?",
                LastMessage = "Thank you for providing your resume for review. Unfortunately, you have not actually uploaded or shared your resume document with me. Without the actual resume file, I am unable to review and provide feedback on its content, formatting, and structure.",
                LastUpdatedTime = "5 hours ago",
                IsSelected = false,
                IsShowConversation = true,
                FavoriteData = _favoriteDatas[0],
                FavoriteColor = _favoriteColors[0],
                SelectConversationCommand = new RelayCommand(ExecuteSelectConversationCommand, o => true),
                EditTitleCommand = new RelayCommand(ExecuteEditTitleCommand, o => true),
                MarkFavoriteCommand = new RelayCommand(ExecuteMarkFavoriteCommand, o => true),
                OpenDeletePopupCommand = new RelayCommand(ExecuteOpenDeletePopupCommand, o => true),
            },

            new ConversationModel
            {
                Title = "4. Manipulating Text Colors",
                LastMessage = "Here is a simple Java code snippet for a Word Count program in Hadoop:",
                LastUpdatedTime = "25/03/2024",
                IsSelected = false,
                IsShowConversation = true,
                FavoriteData = _favoriteDatas[0],
                FavoriteColor = _favoriteColors[0],
                SelectConversationCommand = new RelayCommand(ExecuteSelectConversationCommand, o => true),
                EditTitleCommand = new RelayCommand(ExecuteEditTitleCommand, o => true),
                MarkFavoriteCommand = new RelayCommand(ExecuteMarkFavoriteCommand, o => true),
                OpenDeletePopupCommand = new RelayCommand(ExecuteOpenDeletePopupCommand, o => true),
            },

            new ConversationModel
            {
                Title = "5. Java code for flask in python",
                LastMessage = "This is a basic WordCount program in Hadoop using Java. You can save this code in a file named",
                LastUpdatedTime = "7 days ago",
                IsSelected = false,
                IsShowConversation = true,
                FavoriteData = _favoriteDatas[0],
                FavoriteColor = _favoriteColors[0],
                SelectConversationCommand = new RelayCommand(ExecuteSelectConversationCommand, o => true),
                EditTitleCommand = new RelayCommand(ExecuteEditTitleCommand, o => true),
                MarkFavoriteCommand = new RelayCommand(ExecuteMarkFavoriteCommand, o => true),
                OpenDeletePopupCommand = new RelayCommand(ExecuteOpenDeletePopupCommand, o => true),
            },


        };

        for (int idx = 0; idx < ConversationList.Count; idx++)
        {
            ConversationModel conversation = ConversationList[idx];
            conversation.Idx = idx;
        }

        IsHitTestVisible = true;
    }

    private void FilterConversation()
    {
        string curMessage = AIChatHistorySearchMessage.ToLower();
        foreach (var conversation in ConversationList)
        {
            // Favorite conversations is turned on, this condition will exclude conversations which are not marked as favorite
            if (FilterFavoriteColor == _favoriteColors[1] && conversation.FavoriteColor == _favoriteColors[0])
            {
                continue;
            }

            conversation.IsShowConversation = (curMessage == "") || conversation.Title.ToLower().Contains(curMessage) || conversation.LastMessage.ToLower().Contains(curMessage);
        }
    }

    private async void ExecuteFilterFavoriteChatCommand(object obj)
    {
        string isExecuted = (string)obj;
        if (isExecuted == "change")
        {
            FilterFavoriteColor = isExecuted;     // Switch color
        }

        foreach (var conversation in ConversationList)
        {
            if (FilterFavoriteColor == _favoriteColors[0])
            {
                conversation.IsShowConversation = true;
                continue;
            }

            if (conversation.FavoriteColor == _favoriteColors[0])
            {
                conversation.IsShowConversation = false;
            }
            else
            {
                conversation.IsShowConversation = true;
            }
        }
    }

    private async void ExecuteSelectConversationCommand(object obj)
    {
        int idx = (int)obj;
        foreach (var conversation in ConversationList) { conversation.IsSelected = false; }
        ConversationList[idx].IsSelected = true;
    }
    private async void ExecuteEditTitleCommand(object obj)
    {
        int idx = (int)obj;
        IsTitleEditable = true;
        ConversationIdx = idx;
        Title = ConversationList[ConversationIdx].Title;
    }

    private async void ExecuteMarkFavoriteCommand(object obj)
    {
        int idx = (int)obj;
        ConversationList[idx].IsMarkFavorite = !ConversationList[idx].IsMarkFavorite;
        int i = (ConversationList[idx].IsMarkFavorite) ? 1 : 0;

        ConversationList[idx].FavoriteColor = _favoriteColors[i];
        ConversationList[idx].FavoriteData = _favoriteDatas[i];
        FilterFavoriteChatCommand.Execute("");
    }

    private async void ExecuteOpenDeletePopupCommand(object obj)
    {
        if (obj is null)
        {
            DeleteDescription = "Are you sure you want to delete all\nconversations? The favorited conversations will\nnot be deleted.";
        }
        else
        {
            int idx = (int)obj;
            DeleteDescription = "Are you sure you want to delete this\nconversation?";
            ConversationIdx = idx;

        }

        IsOpenDeletePopup = true;
    }

    private async void ExecuteDeleteCommand(object obj)
    {
        string type = (string)obj;
        if (ConversationIdx == -1)
        {
            int idx = 0;
            while (idx < ConversationList.Count)
            {
                if (!ConversationList[idx].IsMarkFavorite)
                {
                    ConversationList.RemoveAt(idx);
                }
                else
                {
                    ConversationList[idx].Idx = idx;
                    idx++;
                }
            }

            IsOpenDeletePopup = false;

            IsNotEmptyChatHistory = (ConversationList.Count != 0);
            IsEmptyChatHistory = (ConversationList.Count == 0);
            return;
        }

        if (ConversationList.Count == 0)
        {
            return;
        }

        ConversationList.RemoveAt(ConversationIdx);
        for (int i = ConversationIdx; i < ConversationList.Count; i++)
        {
            ConversationList[i].Idx = i;
        }

        IsOpenDeletePopup = false;

        IsNotEmptyChatHistory = (ConversationList.Count != 0);
        IsEmptyChatHistory = (ConversationList.Count == 0);
    }

    private async void ExecuteSaveEditTitleCommand(object obj)
    {
        ConversationList[ConversationIdx].Title = Title;
        IsTitleEditable = false;
    }

}