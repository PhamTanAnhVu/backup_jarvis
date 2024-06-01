using Jarvis_Windows.Sources.DataAccess;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
                string color0 = DataConfiguration.FilterConversationColor(0);
                string color1 = DataConfiguration.FilterConversationColor(1);
                value = (_filterFavoriteColor == color0) ? color1 : color0;
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
            IsEmptyChatHistory = !IsNotEmptyChatHistory;
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
    }

    private void InitConversationList()
    {
        ConversationManager.Instance()._selectedIdx = -1;
        FilterFavoriteColor = DataConfiguration.FilterConversationColor(0);
        IsHitTestVisible = true;

        ConversationList = ConversationManager.Instance().GetAllConversations();
        for (int idx = 0; idx < ConversationList.Count; idx++)
        {
            InitConversation(idx);
        }

        IsNotEmptyChatHistory = (ConversationList.Count != 0);
    }

    public void InitConversation(int conversationIdx)
    {
        var conversation = ConversationManager.Instance().GetConversation(conversationIdx);

        if (conversation.SelectConversationCommand is null)
        {
            conversation.SelectConversationCommand = new RelayCommand(ExecuteSelectConversationCommand, o => true);
            conversation.EditTitleCommand = new RelayCommand(ExecuteEditTitleCommand, o => true);
            conversation.MarkFavoriteCommand = new RelayCommand(ExecuteMarkFavoriteCommand, o => true);
            conversation.OpenDeletePopupCommand = new RelayCommand(ExecuteOpenDeletePopupCommand, o => true);
        }

        if (conversation.IsSelected)
        {
            ConversationManager.Instance()._selectedIdx = conversationIdx;
        }

        if (conversationIdx >= ConversationList.Count)
        {
            ConversationList.Add(conversation);
        }
        else
        {
            ConversationList[conversationIdx] = conversation;
        }

        IsNotEmptyChatHistory = (ConversationList.Count != 0);
    }

    private void FilterConversation()
    {
        string curMessage = AIChatHistorySearchMessage.ToLower();
        foreach (var conversation in ConversationList)
        {
            // Favorite conversations is turned on, this condition will exclude conversations which are not marked as favorite
            if (FilterFavoriteColor == DataConfiguration.FilterConversationColor(1) && conversation.FavoriteColor == DataConfiguration.FilterConversationColor(0))
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
            if (FilterFavoriteColor == DataConfiguration.FilterConversationColor(0))
            {
                conversation.IsShowConversation = true;
                continue;
            }

            if (conversation.FavoriteColor == DataConfiguration.FilterConversationColor(0))
            {
                conversation.IsShowConversation = false;
            }
            else
            {
                conversation.IsShowConversation = true;
            }
        }
    }

    public void DeselectConversation()
    {
        if (ConversationManager.Instance()._selectedIdx == -1) { return; }
        ConversationList[ConversationManager.Instance()._selectedIdx].IsSelected = false;
        ConversationManager.Instance().UpdateConversation(ConversationList[ConversationManager.Instance()._selectedIdx]);
    }

    private async void ExecuteSelectConversationCommand(object obj)
    {
        int idx = (int)obj;
        AIChatSidebarEventTrigger.PublishSelectConversationChanged(idx, EventArgs.Empty);
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

        ConversationList[idx].FavoriteColor = DataConfiguration.FilterConversationColor(i);
        ConversationList[idx].FavoriteData = DataConfiguration.FilterConversationData(i);
        
        ConversationManager.Instance().UpdateConversation(ConversationList[idx]);
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
        if (ConversationIdx == -1)  // Delete all except favorite conversations
        {
            int idx = 0;
            while (idx < ConversationList.Count)
            {
                if (!ConversationList[idx].IsMarkFavorite)
                {
                    ConversationList.RemoveAt(idx);
                    ConversationManager.Instance().DeleteConversation(idx);

                    // Selected conversation is deleted -> set to -1
                    if (ConversationManager.Instance()._selectedIdx == idx)
                    {
                        ConversationManager.Instance()._selectedIdx = -1;
                    }
                }
                else
                {
                    // Update selected conversation idx
                    if (ConversationManager.Instance()._selectedIdx == ConversationList[idx].Idx)
                    {
                        ConversationManager.Instance()._selectedIdx = idx;
                    }
                    ConversationList[idx].Idx = idx;
                    idx++;
                }
            }

            IsOpenDeletePopup = false;
            IsNotEmptyChatHistory = (ConversationList.Count != 0);
            AIChatSidebarEventTrigger.PublishSelectConversationChanged(-1, EventArgs.Empty); // Get new conversation after deletion
            return;
        }

        if (ConversationList.Count == 0)
        {
            return;
        }

        ConversationList.RemoveAt(ConversationIdx);
        if (ConversationManager.Instance()._selectedIdx == ConversationIdx) // Selected conversation is deleted
        {
            ConversationManager.Instance()._selectedIdx = -1;
        }
        ConversationManager.Instance().DeleteConversation(ConversationIdx);
       
        
        AIChatSidebarEventTrigger.PublishSelectConversationChanged(-1, EventArgs.Empty); // Get new conversation after deletion

        for (int i = ConversationIdx; i < ConversationList.Count; i++)
        {
            ConversationList[i].Idx = i;
        }

        IsOpenDeletePopup = false;
        IsNotEmptyChatHistory = (ConversationList.Count != 0);
    }

    private async void ExecuteSaveEditTitleCommand(object obj)
    {
        ConversationList[ConversationIdx].Title = Title;
        ConversationManager.Instance().UpdateConversation(ConversationList[ConversationIdx]);
        IsTitleEditable = false;
    }

    public void UpdateLastUpdatedTime()
    {
        for (int idx = 0; idx < ConversationList.Count; idx++)
        {
            var conversation = ConversationManager.Instance().GetConversation(idx);
            DateTime curDateTime = DateTime.Now;
            TimeSpan timeDifference = curDateTime - conversation.LastUpdatedTimeDT;

            if (timeDifference.TotalSeconds < 60)
            {
                conversation.LastUpdatedTime = "a few seconds ago";
            }
            else if (timeDifference.TotalMinutes < 60)
            {
                int minutes = (int)timeDifference.TotalMinutes;
                conversation.LastUpdatedTime = minutes == 1 ? "a minute ago" : $"{minutes} minutes ago";
            }
            else if (timeDifference.TotalHours < 24)
            {
                int hours = (int)timeDifference.TotalHours;
                conversation.LastUpdatedTime = hours == 1 ? "an hour ago" : $"{hours} hours ago";
            }
            else if (timeDifference.TotalDays <= 7)
            {
                int days = (int)timeDifference.TotalDays;
                conversation.LastUpdatedTime = days == 1 ? "a day ago" : $"{days} days ago";
            }
            else
            {
                conversation.LastUpdatedTime = conversation.LastUpdatedTimeDT.ToString("MMMM dd, yyyy");
            }

            ConversationList[idx].LastUpdatedTime = conversation.LastUpdatedTime;
        }
    }

}