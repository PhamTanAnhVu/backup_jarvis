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

public class MenuSelectionActionsViewModel : ViewModelBase
{
    private SendEventGA4? _googleAnnalyticService;

    private static bool _isMouseOver_AppUI;
    private bool _isMouseOver_TextMenuSelection;
    private bool _isMouseOver_TextMenuPopup;
    private IKeyboardMouseEvents _globalMouseHook;
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
        EventAggregator.MouseOverAppUIChanged += (sender, e) =>
        {
            _isMouseOver_AppUI = (bool)sender;
        };
        EventAggregator.MouseOverTextMenuSelectionChanged += (sender, e) =>
        {
            _isMouseOver_TextMenuSelection = (bool)sender;
        };
        EventAggregator.MouseOverTextMenuPopupChanged += (sender, e) =>
        {
            _isMouseOver_TextMenuPopup = (bool)sender;
        };
    }

    void InitializeServices()
    {
        _googleAnnalyticService = DependencyInjection.GetService<SendEventGA4>();
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
        int textMenuWidth = 32;
        foreach (var action in MenuSelectionButtons)
        {
            action.ExtraIconVisibility = true;
            if (action.Visibility) textMenuWidth += 32;
        }

        PopupDictionaryService.Instance().MenuSelectionPopupListPosition = new System.Drawing.Point
        (
            PopupDictionaryService.Instance().MenuSelectionPopupListPosition.X + (textMenuWidth - 195),
            PopupDictionaryService.Instance().MenuSelectionPopupListPosition.Y + 40
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
        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double xScale = screenWidth / 1920;
        double yScale = screenHeight / 1080;
        Point mousePoint = new Point((int)(e.X * xScale), (int)(e.Y * yScale));

        //TODO::Check if the mouse is not over menu text selection
        if (PopupDictionaryService.Instance().IsShowMenuSelectionActions)
        {
            if (_isMouseOver_TextMenuPopup || _isMouseOver_TextMenuSelection)
            {
                return;
            }

            PopupDictionaryService.Instance().ShowMenuSelectionActions(false);
            PopupDictionaryService.Instance().IsShowMenuSelectionPopupList = false;
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
                    PopupDictionaryService.Instance().ShowMenuSelectionActions(false);
                    return;
                }
                PopupDictionaryService.Instance().ShowMenuSelectionActions(true);
                await _googleAnnalyticService.SendEvent("inject_selection_actions");
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
        if (_isMouseOver_AppUI)
        {
            PopupDictionaryService.Instance().ShowMenuSelectionActions(false);
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
            if (System.Windows.Clipboard.ContainsText())
            {
                AccessibilityService.GetInstance().CurrentSelectedText = Clipboard.GetText();

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
                await _googleAnnalyticService.SendEvent("inject_selection_actions");
            }
            /*else
            {
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }*/
        }
        catch
        {
            PopupDictionaryService.Instance().ShowMenuSelectionActions(false);
            PopupDictionaryService.Instance().ShowSelectionResponseView(false);
        }
    }
}
