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
using System.Diagnostics;
using System.Windows.Automation;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions;

public class MenuSelectionActionsViewModel : ViewModelBase
{
    //private PopupDictionaryService? _popupDictionaryService;
    private UIElementDetector? _accessibilityService;
    private SendEventGA4? _googleAnnalyticService;

    private static bool _isMouseOver_AppUI;
    private bool _isMouseOver_TextMenuSelection;
    private bool _isMouseOver_TextMenuPopup;
    private IKeyboardMouseEvents _globalMouseHook;
    private bool _isMouseOverActions;
    private bool _isMouseOverResponse;
    private bool _isMouseOverPopup;
    public RelayCommand MenuSelectionCommand { get; set; }
    public RelayCommand MenuSelectionPinCommand { get; set; }
    public RelayCommand ShowMenuSelectionPopupListCommand { get; set; }
    public ObservableCollection<AIButton> MenuSelectionButtons { get; set; }

    public MenuSelectionActionsViewModel()
    {
        InitializeServices();
        InitializeMenuSelectionButtons();
        // MenuSelectionCommand = new RelayCommand(ExecuteMenuSelectionCommand, o => true);
        // MenuSelectionPinCommand = new RelayCommand(ExecuteMenuSelectionPinCommand, o => true);
        ShowMenuSelectionPopupListCommand = new RelayCommand(ExecuteShowMenuSelectionPopupListCommand, o => true);
        
        _globalMouseHook = Hook.GlobalEvents();
        _globalMouseHook.MouseDoubleClick += MouseDoubleClicked;
        _globalMouseHook.MouseDragFinished += MouseDragFinished;
        _globalMouseHook.MouseClick += MouseClicked;

        RegisterEvents();
    }

    private void RegisterEvents()
    {
        MenuSelectionSharedData.MouseOverActions += (sender, e) =>
        {
            _isMouseOverActions = (bool)sender;
        };

        MenuSelectionSharedData.MouseOverResponse += (sender, e) =>
        {
            _isMouseOverResponse = (bool)sender;
        };

        MenuSelectionSharedData.MouseOverPopup += (sender, e) =>
        {
            _isMouseOverPopup = (bool)sender;
        };

        MenuSelectionSharedData.MenuSelectionPopupPinExecuted += (sender, e) =>
        {
            int idx = (int)sender;
            MenuSelectionButtons[idx].Visibility = !MenuSelectionButtons[idx].Visibility;
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
            action.Idx = (idx++).ToString();
        }
    }

    private void UpdateMenuSelectionPopupListPosition()
    {
        int selectionActionsWidth = 32;
        foreach (var action in MenuSelectionButtons)
        {
            action.ExtraIconVisibility = true;
            if (action.Visibility) selectionActionsWidth += 32;
        }

        PopupDictionaryService.Instance().MenuSelectionPopupListPosition = new System.Drawing.Point
        (
            PopupDictionaryService.Instance().MenuSelectionActionsPosition.X + (selectionActionsWidth - 190),
            PopupDictionaryService.Instance().MenuSelectionActionsPosition.Y + 40
        );
    }

    public async void ExecuteShowMenuSelectionPopupListCommand(object obj)
    {
        UpdateMenuSelectionPopupListPosition();
        PopupDictionaryService.Instance().IsShowMenuSelectionPopupList = !PopupDictionaryService.Instance().IsShowMenuSelectionPopupList;
        
        if (!PopupDictionaryService.Instance().IsPinMenuSelectionResponse)
        {
            // Turn off MenuSelectionResponse View if not pinned
            PopupDictionaryService.Instance().IsShowMenuSelectionResponse = false;
        }

        if (PopupDictionaryService.Instance().IsShowMenuSelectionPopupList)
        {
            // Turn on PinCommand for buttons in MenuSelectionActions View
            MenuSelectionSharedData.PublishMenuSelectionPopupListExecuted(true, EventArgs.Empty);
        }
    }

    public async void ExecuteMenuSelectionCommand(object obj)
    {
        int idx = 0;
        if (obj is int) { idx = (int)obj; }
        else if (obj is string) { idx = int.Parse(obj.ToString()); }

        MenuSelectionSharedData.PublishMenuSelectionCommandExecuted(MenuSelectionButtons[idx], EventArgs.Empty);
    }

    private void MouseClicked(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        //TODO::Check if the mouse is not over menu selection actions
        if (PopupDictionaryService.Instance().IsShowMenuSelectionActions && !_isMouseOverActions)
        {
            PopupDictionaryService.Instance().IsShowMenuSelectionActions = false;
        }

        if (PopupDictionaryService.Instance().IsShowMenuSelectionPopupList && !_isMouseOverPopup)
            {
            PopupDictionaryService.Instance().IsShowMenuSelectionPopupList = false;
            }


        if (PopupDictionaryService.Instance().IsShowMenuSelectionResponse && !PopupDictionaryService.Instance().IsPinMenuSelectionResponse && !_isMouseOverResponse)
        {
            PopupDictionaryService.Instance().IsShowMenuSelectionResponse = false;
        }
    }
    private async void MouseDoubleClicked(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        return;
        IDataObject tmpClipboard = System.Windows.Clipboard.GetDataObject();
        System.Windows.Clipboard.Clear();
        await Task.Delay(50);

        // Send Ctrl+C, which is "copy"
        System.Windows.Forms.SendKeys.SendWait("^c");

        await Task.Delay(50);

        try
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                AccessibilityService.GetInstance().CurrentSelectedText = text;
                if (PopupDictionaryService.Instance().IsPinMenuSelectionResponse && PopupDictionaryService.Instance().IsShowMenuSelectionResponse)
                {
                    PopupDictionaryService.Instance().IsShowMenuSelectionActions = false;
                    return;
                }
                PopupDictionaryService.Instance().ShowMenuSelectionActions(true);
                _ = GoogleAnalyticService.Instance().SendEvent("inject_selection_actions");
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }
        }
        catch { }
    }

    private async Task<IDataObject?> RetryGetClipboardObject()
    {
        IDataObject? tmpClipboard = null;
        int retries = 10;
        while (retries > 0)
        {
            try
            {
                tmpClipboard = System.Windows.Clipboard.GetDataObject();
                break;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                if (retries == 0)
                {
                    // rethrow the exception if no retries are left
                    throw;
                }
                // wait for some time and retry
                await Task.Delay(0);
                retries--;
            }
        }
        return tmpClipboard;
    }

    static System.Windows.Point _lastMousePoint = new System.Windows.Point();
    private async void MouseDragFinished(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        AutomationElement rootElement = AutomationElement.FromHandle(NativeUser32API.GetForegroundWindow());
        if (PopupDictionaryService.Instance().IsDragging || rootElement.Current.Name.Equals("MainNavigationView"))
        {
            PopupDictionaryService.Instance().IsShowMenuSelectionActions = false;
            return;
        }

        System.Windows.Point currentMousePoint = new System.Windows.Point(e.X, e.Y);
        double distance = (currentMousePoint - _lastMousePoint).Length;
        if (distance < 5)
        {
            _lastMousePoint = currentMousePoint;
            return;
        }
        _lastMousePoint = currentMousePoint;

        IDataObject? tmpClipboard = RetryGetClipboardObject().Result;
        if (tmpClipboard == null) return;
        System.Windows.Clipboard.Clear();

        await Task.Delay(50);
        System.Windows.Forms.SendKeys.SendWait("^c");

        try
        {
            if (!_isMouseOverResponse && System.Windows.Clipboard.ContainsText())
            {
                AccessibilityService.GetInstance().CurrentSelectedText = Clipboard.GetText();
                Debug.WriteLine("Clipboard text: " + Clipboard.GetText());

                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double xScale = screenWidth / 1920;
                double yScale = screenHeight / 1080;
                System.Drawing.Point lpPoint;
                NativeUser32API.GetCursorPos(out lpPoint);
                Point selectedTextPosition = new Point((int)(lpPoint.X * xScale), (int)(lpPoint.Y * yScale));
                PopupDictionaryService.Instance().MenuSelectionActionsPosition = new System.Drawing.Point
                (
                    (int) selectedTextPosition.X, 
                    (int) selectedTextPosition.Y + 10
                );
                Point newPosition = new Point(selectedTextPosition.X, selectedTextPosition.Y + 50);
                PopupDictionaryService.Instance().MenuSelectionPopupListPosition = new System.Drawing.Point
                (
                    (int)newPosition.X, 
                    (int)newPosition.Y
                );

                if (!PopupDictionaryService.Instance().IsPinMenuSelectionResponse)
                {
                    PopupDictionaryService.Instance().ShowSelectionResponseView(false);
                    PopupDictionaryService.Instance().MenuSelectionResponsePosition = new System.Drawing.Point
                    (
                        (int)newPosition.X, 
                        (int)newPosition.Y
                    );
                }

                PopupDictionaryService.Instance().ShowMenuSelectionActions(true);
                _ = GoogleAnalyticService.Instance().SendEvent("inject_selection_actions");
            }
            /*else
            {
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }*/
        }
        catch
        {
            PopupDictionaryService.Instance().IsShowMenuSelectionActions = false;
            PopupDictionaryService.Instance().ShowSelectionResponseView(false);
        }
    }
}
