using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System.Collections.ObjectModel;
using Gma.System.MouseKeyHook;
using System.Threading.Tasks;
using System.Windows;
using System;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions;

public class MenuSelectionPopupListViewModel : ViewModelBase
{
    private AccessibilityService? _accessibilityService;
    private SendEventGA4? _googleAnnalyticService;
    public ObservableCollection<AIButton> MenuSelectionButtons { get; set; }
    public MenuSelectionPopupListViewModel()
    {
        InitializeMenuSelectionButtons();
        
        MenuSelectionSharedData.MenuSelectionPopupListExecuted += (sender, e) =>
        {
            bool isContainPinButton = (bool)sender;
            foreach (var action in MenuSelectionButtons)
            {
                action.ExtraIconVisibility = isContainPinButton;
            }
        };
    }

    private void InitializeMenuSelectionButtons()
    {
        AIActionTemplate aIActionTemplate = new AIActionTemplate();
        MenuSelectionButtons = aIActionTemplate.MenuSelectionButtonList;
        int idx = 0;
        foreach (var action in MenuSelectionButtons)
        {
            action.Command = new RelayCommand(ExecuteMenuSelectionCommand, o => true);
            action.PinCommand = new RelayCommand(ExecuteMenuSelectionPinCommand, o => true);
            action.Idx = (idx++).ToString();
            action.ExtraIconVisibility = true;
            if (action.PinColor == "") action.PinColor = "Transparent";
        }
    }

    public async void ExecuteMenuSelectionCommand(object obj)
    {
        int idx = 0;
        if (obj is int) { idx = (int)obj; }
        else if (obj is string) { idx = int.Parse(obj.ToString()); }

        MenuSelectionSharedData.PublishMenuSelectionCommandExecuted(MenuSelectionButtons[idx], EventArgs.Empty);
    }

    private async void ExecuteMenuSelectionPinCommand(object obj)
    {
        string[] colors = { "Transparent", "#6841EA" };
        int idx = 0;

        if (obj is string) { idx = int.Parse(obj.ToString()); }

        bool visibilityStatus = !MenuSelectionButtons[idx].Visibility;
        int sizeChanged = (visibilityStatus) ? -32 : 32;
        MenuSelectionButtons[idx].Visibility = visibilityStatus;

        // Add buttons and expand to the left, the problem is the UI will reload everytime -> not smooth
        PopupDictionaryService.Instance().MenuSelectionActionsPosition = new System.Drawing.Point(
            PopupDictionaryService.Instance().MenuSelectionActionsPosition.X + sizeChanged,
            PopupDictionaryService.Instance().MenuSelectionActionsPosition.Y
        );

        //UpdateMenuSelectionPopupListPosition();

        PopupDictionaryService.Instance().IsShowMenuSelectionActions = true;
        PopupDictionaryService.Instance().IsShowMenuSelectionPopupList = true;
        MenuSelectionButtons[idx].PinColor = colors[Convert.ToInt32(visibilityStatus)];
        OnPropertyChanged(nameof(MenuSelectionButtons));
    }
}
