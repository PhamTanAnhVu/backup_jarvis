using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Collections.ObjectModel;
using Windows.Media.AppBroadcasting;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;

public class AIChatBubbleViewModel : ViewModelBase
{
    private PopupDictionaryService _popupDictionaryService;
    public RelayCommand ShowMainNavigationCommand { get; set; }
    public RelayCommand CloseAIBubbleCommand { get; set; }
    private ObservableCollection<AIBubbleButton> _aIBubbleButton;
    public ObservableCollection<AIBubbleButton> AIBubbleButton
    {
        get { return _aIBubbleButton; }
        set
        {
            _aIBubbleButton = value;
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

    public AIChatBubbleViewModel()
    {
        ShowMainNavigationCommand = new RelayCommand(ExecuteShowMainNavigationCommand, o => true);
        CloseAIBubbleCommand = new RelayCommand(ExecuteCloseAIBubbleCommand, o => true);
        InitAIBubbleButton();
    }

    private PropertyMessage InitPropertyMessage(string propertyName, object value)
    {
        return new PropertyMessage(propertyName, value);
    }

    public async void ExecuteShowMainNavigationCommand(object obj)
    {
        PopupDictionaryService.Instance().IsShowMainNavigation = true;
        EventAggregator.PublishPropertyMessageChanged(
            InitPropertyMessage("IsShowMainNavigation", true), null
        );
    }
    public async void ExecuteCloseAIBubbleCommand(object obj)
    {
        PopupDictionaryService.Instance().IsShowAIChatBubble = false;
        EventAggregator.PublishPropertyMessageChanged(
            InitPropertyMessage("IsShowAIChatBubble", false), null
        );
    }

    private void InitAIBubbleButton()
    {
        AIBubbleButton = new ObservableCollection<AIBubbleButton>();
        for (int i = 0; i < 5; i++)
        {
            AIBubbleButton.Add(new AIBubbleButton()
            {
                Color = "#334155"
            });
        }

        AIChatBubbleEventTrigger.HoverExtraButtonEvent += (sender, e) =>
        {
            string[] content = ((string)sender).Split(" ");
            int idx = int.Parse(content[0]);
            string status = content[1];

            AIBubbleButton[idx].Color = (status == "True") ? "#0078D4" : "#334155";
        };
    }
}
