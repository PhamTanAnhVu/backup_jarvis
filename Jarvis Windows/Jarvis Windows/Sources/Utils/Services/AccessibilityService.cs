using System;
using System.Windows.Automation;
using System.Windows;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Automation.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using Windows.Graphics.Printing3D;
using System.IO;
using Jarvis_Windows.Sources.DataAccess.Local;
using System.Windows.Media.Imaging;
//using System.Drawing;
using System.Windows.Media.Animation;
using Point = System.Drawing.Point;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Windows.Foundation.Diagnostics;

namespace Jarvis_Windows.Sources.Utils.Accessibility;

public class UIElementDetector
{
    private static UIElementDetector? Instance;
    private static AutomationElement? _focusingElement;
    private static AutomationFocusChangedEventHandler? _focusChangedEventHandler = new AutomationFocusChangedEventHandler(OnElementFocusChanged);

    //private static PopupDictionaryService? _popupDictionaryService;
    private static SendEventGA4? _sendEventGA4;
    private static bool _isUseAutoTuningPosition = true;
    private static IAutomationElementValueService? _automationElementValueService;
    private static string _currentSelectedText = String.Empty;
    private static AutomationElement? _observerSelectionChangeElement;
    private static ISupportedAppService? _supportedAppSerice;

    private static bool _isMouseOverAppUI;
    private static bool _isMouseOverAIChatPanel;

    public static bool IsUseAutoTuningPosition { 
        get => _isUseAutoTuningPosition; 
        set => _isUseAutoTuningPosition = value; 
    }

    public AutomationFocusChangedEventHandler? FocusChangedEventHandler
    {
        get { return _focusChangedEventHandler; }
        set => _focusChangedEventHandler = value;
    }

    public static UIElementDetector GetInstance()
    {
        if (Instance == null)
            Instance = new UIElementDetector();
        return Instance;
    }

    public SendEventGA4? SendEventGA4
    {
        get { return _sendEventGA4; }
        set => _sendEventGA4 = value;
    }

    public IAutomationElementValueService? AutomationElementValueService { 
        get => _automationElementValueService;
        set => _automationElementValueService = value; 
    }
    public static string CurrentSelectedText
    {
        get => _currentSelectedText; 
        set => _currentSelectedText = value; 
    }
    public static AutomationElement? ObserverSelectionChangeElement 
    {
        get => _observerSelectionChangeElement; 
        set => _observerSelectionChangeElement = value; 
    }
    /*public PopupDictionaryService? PopupDictionaryService 
    { 
        get => _popupDictionaryService; 
        set => _popupDictionaryService = value; 
    }*/
    public ISupportedAppService SupportedAppService 
    { 
        get => _supportedAppSerice;
        set => _supportedAppSerice = value;
    }

    public UIElementDetector(/*PopupDictionaryService popupDictionaryService*/)
    {
        //PopupDictionaryService = popupDictionaryService;
        EventAggregator.MouseOverAppUIChanged += (sender, e) => {
            _isMouseOverAppUI = (bool)sender;
        };
    }

    /*public UIElementDetector()
    { }*/

    public void SubscribeToElementFocusChanged()
    {
        Automation.AddAutomationFocusChangedEventHandler(_focusChangedEventHandler);
        /*Thread tuningJarivsPositionThread = new Thread(TunningPositionThread);
        tuningJarivsPositionThread.Name = "Jarvis Position Tuning";
        tuningJarivsPositionThread.Start();*/
    }

    private void TunningPositionThread(object? obj)
    {
        /*if (_popupDictionaryService != null)
        {
            try
            {
                while (true)
                {
                    if (_focusingElement != null && IsUseAutoTuningPosition)
                    {
                        _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation());
                        _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation());
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception)
            {
            }
        }*/
    }

    public void UnSubscribeToElementFocusChanged()
    {
        _focusChangedEventHandler = new AutomationFocusChangedEventHandler(OnElementFocusChanged);
        Automation.RemoveAutomationFocusChangedEventHandler(_focusChangedEventHandler);
    }

    private static bool IsEditableElement(AutomationElement? automationElement)
    {
        if (automationElement != null)
        {
            if (automationElement.Current.ControlType.ProgrammaticName.Equals("ControlType.Edit"))
                return true;
            else if (automationElement.Current.ControlType.ProgrammaticName.Equals("ControlType.Custom"))
                return true;
        }
        return false;
    }

    private static void ExecuteSendEventInject()
    {   
        if (_sendEventGA4 != null)
            _ = _sendEventGA4.SendEvent("inject_input_actions");       
    }

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    private static string GetActiveWindowTitle()
    {
        const int nChars = 256;
        StringBuilder Buff = new StringBuilder(nChars);
        IntPtr handle = NativeUser32API.GetForegroundWindow();

        if (GetWindowText(handle, Buff, nChars) > 0)
        {
            return Buff.ToString();
        }
        return String.Empty;
    }

    private static void OnElementFocusChanged(object sender, AutomationFocusChangedEventArgs e)
    {
        AutomationElement? newFocusElement = sender as AutomationElement;
        
        /*string appName = GetActiveWindowTitle();
        if (_supportedAppSerice != null)
        {
            if (!_supportedAppSerice.IsSupportedInjectionApp(appName) &&
                !string.IsNullOrEmpty(appName) && appName != "Jarvis MainView")
            {
                _popupDictionaryService.ShowJarvisAction(false);
                _popupDictionaryService.ShowMenuOperations(false);
                return;
            }
        }*/

        if (newFocusElement != null && newFocusElement != _focusingElement)
        {
            if (newFocusElement.Current.AutomationId.Equals("Jarvis_Custom_Action_TextBox") ||
                newFocusElement.Current.AutomationId.Equals("AIChatSidebar_InputTextbox"))
            {
                PopupDictionaryService.Instance().ShowJarvisAction(false);
                //_popupDictionaryService.ShowMenuOperations(false);
                return;
            }


            if (IsEditableElement(newFocusElement))
            {
                _focusingElement = newFocusElement;
                if(/*PopupDictionaryService.Instance() != null && */_automationElementValueService != null)
                {
                    PopupDictionaryService.Instance().ShowJarvisAction(true);
                    PopupDictionaryService.Instance().ShowMenuOperations(false);
                    PopupDictionaryService.Instance().UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                    PopupDictionaryService.Instance().UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                    //_popupDictionaryService.MainWindow.ResetBinding();
                    ExecuteSendEventInject();
                    _automationElementValueService.CheckUndoRedo(_focusingElement);
                }
            }
            else
            {
                try
                {
                    IntPtr currentAppHandle = NativeUser32API.GetForegroundWindow();
                    AutomationElement foregroundApp = AutomationElement.FromHandle(currentAppHandle);
                    if (foregroundApp != null)
                    {
                        if (foregroundApp.Current.Name.Equals("Jarvis MainView"))
                            return;
                        if (foregroundApp.Current.ClassName.Equals("Popup"))
                            return;
                    }

                    _focusingElement = null;
                    PopupDictionaryService.Instance().ShowJarvisAction(false);
                    PopupDictionaryService.Instance().ShowMenuOperations(false);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    private static Rect GetElementRectBounding(AutomationElement? automationElement)
    {
        if (automationElement == null)
            throw new NullReferenceException("GetElementRectBouding failed !!!");

        return automationElement.Current.BoundingRectangle;
    }

    private static Point CalculateElementLocation()
    {
        Point placementPoint = new Point(0, 0);
        if (_focusingElement != null)
        {
            try
            {
                Rect elementRectBounding = _focusingElement.Current.BoundingRectangle;
                if(elementRectBounding.X <0 || elementRectBounding.Y < 0)
                {
                    PopupDictionaryService.Instance().ShowJarvisAction(false);
                    PopupDictionaryService.Instance().ShowMenuOperations(false);
                    placementPoint.X = 0;
                    placementPoint.Y = 0;
                }
                else
                {
                    placementPoint.X = (int)(elementRectBounding.Left/* + elementRectBounding.Width*/);
                    placementPoint.Y = (int)(elementRectBounding.Top /*+ elementRectBounding.Height * 0.5*/);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        return placementPoint;
    }

    private void SubscribeToElementPropertyChanged(AutomationElement? trackingElement, AutomationProperty? trackingProperty)
    {
        try
        {
            if (trackingElement != null)
            {
                TreeScope treeScope = TreeScope.Element;
                if (trackingProperty == AutomationElement.BoundingRectangleProperty)
                    treeScope = TreeScope.Parent;
                else if (trackingProperty == AutomationElement.IsOffscreenProperty)
                    treeScope = TreeScope.Ancestors;
                else if (trackingProperty == AutomationElement.IsKeyboardFocusableProperty)
                    treeScope = TreeScope.Element;

                AutomationPropertyChangedEventHandler propertyChangedHandler = new AutomationPropertyChangedEventHandler(OnElementPropertyChanged);
                Automation.AddAutomationPropertyChangedEventHandler(trackingElement, treeScope, propertyChangedHandler, trackingProperty);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
            PopupDictionaryService.Instance().ShowJarvisAction(false);
            PopupDictionaryService.Instance().ShowMenuOperations(false);
        }
    }

    private void OnElementPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
    {
        var automationElement = sender as AutomationElement;
        if (e.Property == AutomationElement.BoundingRectangleProperty)
        {
            Debug.WriteLine($"🟧🟧🟧 {automationElement?.Current.Name} Bounding Rectangle Changed");
            PopupDictionaryService.Instance().UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
            PopupDictionaryService.Instance().UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
        }
        else if (e.Property == AutomationElement.IsOffscreenProperty)
        {
            Debug.WriteLine($"👁️👁️👁️ {automationElement?.Current.ControlType.ProgrammaticName} Offscreen Property Changed");
            PopupDictionaryService.Instance().ShowJarvisAction(false);
            PopupDictionaryService.Instance().ShowMenuOperations(false);
        }
    }

    private AutomationElement FindChildEditElement(AutomationElement parrentElement)
    {
        System.Windows.Automation.Condition editCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit);

        AutomationElement editElement = parrentElement.FindFirst(TreeScope.Descendants, editCondition);

        return editElement;
    }

    public string SetValueForFocusingEditElement(String? value)
    {
        int timeoutMilliseconds = 200;
        string strResult = String.Empty;
        if (_focusingElement != null)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task setValueTask = Task.Run(() =>
            {
                try
                {
                    ValuePattern? valuePattern = _focusingElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    if (valuePattern != null)
                    {
                        strResult = valuePattern.Current.Value.ToString();

                        if(String.IsNullOrEmpty(CurrentSelectedText))
                        {
                            valuePattern.SetValue(value);
                        }
                        else
                        {
                            strResult = strResult.Replace(CurrentSelectedText, value);
                            valuePattern.SetValue(strResult);
                            CurrentSelectedText = String.Empty;
                        }
                    }    
                }
                catch (Exception)
                {
                    throw;
                }
            });

            if (!setValueTask.Wait(timeoutMilliseconds, cancellationToken))
            {
                cancellationTokenSource.Cancel();
                return String.Empty;
            }
        }
        return strResult;
    }

    public string GetTextFromFocusingEditElement()
    {
        int timeoutMilliseconds = 200;
        string result = string.Empty;

        if (_focusingElement != null)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task<string> getValueTask = Task.Run(() =>
            {
                Debug.WriteLine($"??? Get Value of {_focusingElement.Current.ClassName} {_focusingElement.Current.ControlType.ProgrammaticName}");
                try
                {
                    ValuePattern? valuePattern = null;
                    object valuePatternObj;
                    if (_focusingElement.TryGetCurrentPattern(ValuePattern.Pattern, out valuePatternObj))
                    {
                        valuePattern = valuePatternObj as ValuePattern;
                        if (valuePattern != null)
                            return valuePattern.Current.Value;
                    }
                }
                catch (NullReferenceException)
                {
                    Debug.WriteLine($"Null reference exception");
                }
                catch (ElementNotAvailableException)
                {
                    Debug.WriteLine($"??? Element is not available");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"??? Exception: {ex.Message}");
                }
                return string.Empty;
            });

            if (!getValueTask.Wait(timeoutMilliseconds, cancellationToken))
            {
                cancellationTokenSource.Cancel();
                throw new TimeoutException("The GetTextFromFocusingEditElement operation has timed out.");
            }

            result = getValueTask.Result;
        }

        return result;
    }

    internal AutomationElement? GetFocusingElement()
    {
        return _focusingElement;
    }

    internal string GetValueOf(AutomationElement? automationElement)
    {
        int timeoutMilliseconds = 200;
        string result = string.Empty;

        if (automationElement != null)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task<string> getValueTask = Task.Run(() =>
            {
                try
                {
                    ValuePattern? valuePattern = null;
                    object valuePatternObj;
                    if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out valuePatternObj))
                    {
                        valuePattern = valuePatternObj as ValuePattern;
                        if (valuePattern != null)
                            return valuePattern.Current.Value;
                    }
                }
                catch (NullReferenceException)
                {
                    Debug.WriteLine($"Null reference exception");
                }
                catch (ElementNotAvailableException)
                {
                    Debug.WriteLine($"??? Element is not available");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"??? Exception: {ex.Message}");
                }
                return string.Empty;
            });

            if (!getValueTask.Wait(timeoutMilliseconds, cancellationToken))
            {
                cancellationTokenSource.Cancel();
                throw new TimeoutException("The GetTextFromFocusingEditElement operation has timed out.");
            }

            result = getValueTask.Result;
        }

        return result;
    }

    private static void OnTextSelectionChange(object sender, AutomationEventArgs e)
    {
        return;
        AutomationElement? automationElement = sender as AutomationElement;
        if (automationElement != null && 
            !automationElement.Current.AutomationId.Equals("TextMenuAPI_Result_Text") &&
            !automationElement.Current.AutomationId.Equals("AIChatSidebar_ChatPanel") &&
            !automationElement.Current.AutomationId.Equals("AIChatSidebar_InputTextbox") &&
            !automationElement.Current.AutomationId.Equals("Jarvis_Custom_Action_TextBox") &&
            !_isMouseOverAppUI)
        {      
            try
            {
                TextPattern? textPattern;
                if (automationElement.TryGetCurrentPattern(TextPattern.Pattern, out object textPatternObj))
                {
                    textPattern = textPatternObj as TextPattern;
                    if (textPattern != null)
                    {
                        if (textPattern.SupportedTextSelection == SupportedTextSelection.None)
                        {
                            //throw new NotSupportedException("TextSelection is not supported for this element.");
                        }
                        else
                        {
                            TextPatternRange[] selectedRanges = textPattern.GetSelection();
                            if (selectedRanges != null && selectedRanges.Length > 0)
                            {
                                TextPatternRange selection = selectedRanges[0];
                                if (selection != null)
                                {
                                    Rect[] rects;
                                    try 
                                    { 
                                        rects = selection.GetBoundingRectangles(); 
                                    } 
                                    catch { return; }
                                    if (rects != null && rects.Length > 0)
                                    {
                                        Rect boundingRect = rects[0];
                                        /*if (boundingRect == Rect.Empty)
                                        {
                                            _popupDictionaryService.ShowMenuSelectionActions(false);
                                            _popupDictionaryService.IsShowMenuSelectionPopupList = false;
                                            return;
                                        }*/
                                        /*_popupDictionaryService.ShowMenuSelectionActions(false);
                                        _popupDictionaryService.IsShowMenuSelectionPopupList = false;*/
                                        double screenHeight = SystemParameters.PrimaryScreenHeight;
                                        double screenWidth = SystemParameters.PrimaryScreenWidth;
                                        double xScale = screenWidth / 1920;
                                        double yScale = screenHeight / 1080;

                                        System.Drawing.Point lpPoint;
                                        NativeUser32API.GetCursorPos(out lpPoint);
                                        Debug.WriteLine($"?????? Cursor Position: {lpPoint.X} - {lpPoint.Y}");
                                        //Point selectedTextPosition = new Point(boundingRect.X * xScale - 20, boundingRect.Y * yScale + boundingRect.Height * 1.5f);
                                        
                                        Point selectedTextPosition = new Point((int)(lpPoint.X * xScale), (int)(lpPoint.Y * yScale));
                                        PopupDictionaryService.Instance().TextMenuOperationsPosition = new Point(selectedTextPosition.X, selectedTextPosition.Y + 10);
                                    
                                        if (!PopupDictionaryService.Instance().IsShowPinTextMenuAPI)
                                        {
                                            PopupDictionaryService.Instance().IsShowTextMenuAPI = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CurrentSelectedText = String.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public string GetSelectedText()
    {
        return CurrentSelectedText;
    }

    private static void RegisterSelectionChangedFor(AutomationElement automationElement)
    {
        //if(ObserverSelectionChangeElement != null)
        //Automation.RemoveAutomationEventHandler(TextPattern.TextSelectionChangedEvent, ObserverSelectionChangeElement, OnTextSelectionChange);
        try
        {
            ObserverSelectionChangeElement = automationElement;
            Automation.AddAutomationEventHandler(TextPattern.TextSelectionChangedEvent, ObserverSelectionChangeElement, TreeScope.Element, OnTextSelectionChange);
        }
        catch { }
    }

    /*private string GetTextFromFocusedControl()
    {
        try
        {
            int activeWinPtr = GetForegroundWindow().ToInt32();
            int activeThreadId = 0, processId;
            activeThreadId = GetWindowThreadProcessId(activeWinPtr, out processId);
            int currentThreadId = GetCurrentThreadId();
            if (activeThreadId != currentThreadId)
                AttachThreadInput(activeThreadId, currentThreadId, true);
            IntPtr activeCtrlId = GetFocus();

            return GetText(activeCtrlId);
        }
        catch (Exception exp)
        {
            return exp.Message;
        }
    }

    private string GetText(IntPtr handle)
    {
        int maxLength = 100;
        IntPtr buffer = Marshal.AllocHGlobal((maxLength + 1) * 2);
        SendMessageW(handle, WM_GETTEXT, maxLength, buffer);
        string w = Marshal.PtrToStringUni(buffer);
        Marshal.FreeHGlobal(buffer);
        return w;
    }*/
}
