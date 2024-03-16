using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;

public class AIChatBubbleViewModel : ViewModelBase
{
    private PopupDictionaryService _popupDictionaryService;
    public RelayCommand ShowAISidebarCommand { get; set; }

    public PopupDictionaryService PopupDictionaryService
    {
        get { return _popupDictionaryService; }
        set
        {
            _popupDictionaryService = value;
            OnPropertyChanged();
        }
    }

    public AIChatBubbleViewModel(PopupDictionaryService popupDictionaryService)
    {
        PopupDictionaryService = popupDictionaryService;

        ShowAISidebarCommand = new RelayCommand(ExecuteShowAISidebarCommand, o => true);
    }

    public async void ExecuteShowAISidebarCommand(object obj)
    {
        PopupDictionaryService.ShowAIChatSidebar(true);
        PopupDictionaryService.ShowAIChatBubble(false);
    }
}
