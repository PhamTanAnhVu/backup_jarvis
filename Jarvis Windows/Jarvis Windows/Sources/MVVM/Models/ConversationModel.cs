using Jarvis_Windows.Sources.Utils.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class ConversationModel : ViewModelBase
{
    private string _title;
    private string _lastUpdatedTime;
    private string _lastMessage;
    private string _favoriteColor;
    private string _favoriteData;
    private bool _isSelected;
    private bool _isShowConversation;
    private bool _isMarkFavorite;
    private int _idx;
    private ObservableCollection<AIChatMessage> _chatMessages;

    [Ignore]
    public RelayCommand SelectConversationCommand { get; set; }
    [Ignore]
    public RelayCommand EditTitleCommand { get; set; }
    [Ignore]
    public RelayCommand MarkFavoriteCommand { get; set; }
    [Ignore]
    public RelayCommand OpenDeletePopupCommand { get; set; }
    [Ignore]
    public RelayCommand DeleteCommand { get; set; }

    public DateTime LastUpdatedTimeDT { get; set; }
    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public string LastUpdatedTime
    {
        get { return _lastUpdatedTime; }
        set
        {
            _lastUpdatedTime = value;
            OnPropertyChanged();
        }
    }

    public string LastMessage
    {
        get { return _lastMessage; }
        set
        {
            _lastMessage = value;
            OnPropertyChanged();
        }
    }
    public string FavoriteColor
    {
        get { return _favoriteColor; }
        set
        {
            _favoriteColor = value;
            OnPropertyChanged();
        }
    }
    public string FavoriteData
    {
        get { return _favoriteData; }
        set
        {
            _favoriteData = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowConversation
    {
        get { return _isShowConversation; }
        set
        {
            _isShowConversation = value;
            OnPropertyChanged();
        }
    }
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }
    public bool IsMarkFavorite
    {
        get { return _isMarkFavorite; }
        set
        {
            _isMarkFavorite = value;
            OnPropertyChanged();
        }
    }

    [PrimaryKey]
    public int Idx
    {
        get { return _idx; }
        set
        {
            _idx = value;
            OnPropertyChanged();
        }
    }

    [Ignore]
    public ObservableCollection<AIChatMessage> ChatMessages
    {
        get { return _chatMessages; }
        set
        {
            _chatMessages = value;
            OnPropertyChanged();
        }
    } 
}