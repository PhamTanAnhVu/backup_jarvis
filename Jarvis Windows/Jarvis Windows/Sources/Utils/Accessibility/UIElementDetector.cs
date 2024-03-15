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

namespace Jarvis_Windows.Sources.Utils.Accessibility;

public class UIElementDetector
{
    private static UIElementDetector? Instance;
    private static AutomationElement? _focusingElement;
    private static AutomationFocusChangedEventHandler? _focusChangedEventHandler = new AutomationFocusChangedEventHandler(OnElementFocusChanged);

    private static PopupDictionaryService? _popupDictionaryService;
    private static SendEventGA4? _sendEventGA4;
    private static bool _isUseAutoTuningPosition = true;
    private static IAutomationElementValueService? _automationElementValueService;
    private static string _currentSelectedText = String.Empty;
    private static AutomationElement? _observerSelectionChangeElement;

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
    public PopupDictionaryService? PopupDictionaryService 
    { 
        get => _popupDictionaryService; 
        set => _popupDictionaryService = value; 
    }

    private UIElementDetector(PopupDictionaryService popupDictionaryService)
    {
        PopupDictionaryService = popupDictionaryService;
    }

    public UIElementDetector()
    {
    }

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

    private static async Task ExecuteSendEventInject()
    {   
        await _sendEventGA4.SendEvent("inject_input_actions");       
    }

    private static void OnElementFocusChanged(object sender, AutomationFocusChangedEventArgs e)
    {
        AutomationElement? newFocusElement = sender as AutomationElement;
        if (newFocusElement != null)
            RegisterSelectionChangedFor(newFocusElement);

        AutomationElement? editElement = FindFirstElementType(newFocusElement, ControlType.Group);
        if (editElement != null)
        {
            Debug.WriteLine($"EDIT ELEMENT {editElement.Current.Name}");
        }

        if (newFocusElement != null && newFocusElement != _focusingElement)
        {
            if (newFocusElement.Current.AutomationId.Equals("Jarvis_Custom_Action_TextBox") ||
                newFocusElement.Current.ControlType.ProgrammaticName.Equals("ControlType.Window"))
                return;

            if (IsEditableElement(newFocusElement))
            {
                _focusingElement = newFocusElement;
                // Publish Jarvis Action Position to EventAggregator
                EventAggregator.PublishJarvisActionPositionChanged(_focusingElement.Current.AutomationId, EventArgs.Empty);
                _popupDictionaryService.ShowJarvisAction(true);
                _popupDictionaryService.ShowMenuOperations(false);
                _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                _popupDictionaryService.MainWindow.ResetBinding();
                Debug.WriteLine("📩📩📩 Send GA4 Events Inject");
                Task.Run(async () => await ExecuteSendEventInject());

                _automationElementValueService.CheckUndoRedo(_focusingElement);
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
                    _popupDictionaryService.ShowJarvisAction(false);
                    _popupDictionaryService.ShowMenuOperations(false);
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
                    _popupDictionaryService.ShowJarvisAction(false);
                    _popupDictionaryService.ShowMenuOperations(false);
                    placementPoint.X = 0;
                    placementPoint.Y = 0;
                }
                else
                {
                    placementPoint.X = elementRectBounding.Left + elementRectBounding.Width;
                    placementPoint.Y = elementRectBounding.Top + elementRectBounding.Height * 0.5;
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
        catch (NullReferenceException)
        {
            Debug.WriteLine($"Null reference exception");
            PopupDictionaryService.ShowJarvisAction(false);
            PopupDictionaryService.ShowMenuOperations(false);
        }
        catch (ElementNotAvailableException)
        {
            Debug.WriteLine($"Element is not available");
            PopupDictionaryService.ShowJarvisAction(false);
            PopupDictionaryService.ShowMenuOperations(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
            PopupDictionaryService.ShowJarvisAction(false);
            PopupDictionaryService.ShowMenuOperations(false);
        }
    }

    private void OnElementPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
    {
        var automationElement = sender as AutomationElement;
        if (e.Property == AutomationElement.BoundingRectangleProperty)
        {
            Debug.WriteLine($"🟧🟧🟧 {automationElement?.Current.Name} Bounding Rectangle Changed");
            PopupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
            PopupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));

            PopupDictionaryService.UpdateTextMenuOperationsPosition(CalculateElementLocation());
            PopupDictionaryService.UpdateTextMenuAPIPosition();

        }
        else if (e.Property == AutomationElement.IsOffscreenProperty)
        {
            Debug.WriteLine($"👁️👁️👁️ {automationElement?.Current.ControlType.ProgrammaticName} Offscreen Property Changed");
            PopupDictionaryService.ShowJarvisAction(false);
            PopupDictionaryService.ShowMenuOperations(false);
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
                Debug.WriteLine($"❌❌❌ Get Value of {_focusingElement.Current.ClassName} {_focusingElement.Current.ControlType.ProgrammaticName}");
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
                    Debug.WriteLine($"❌❌❌ Element is not available");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌❌❌ Exception: {ex.Message}");
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
                    Debug.WriteLine($"❌❌❌ Element is not available");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌❌❌ Exception: {ex.Message}");
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

        AutomationElement? automationElement = sender as AutomationElement;
        if (automationElement != null)
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
                                    var rects = selection.GetBoundingRectangles();
                                    if (rects != null && rects.Length > 0)
                                    {
                                        Rect boundingRect = rects[0];
                                        _popupDictionaryService.ShowMenuSelectionActions(true);
                                        _popupDictionaryService.ShowSelectionResponseView(false);
                                        double screenHeight = SystemParameters.PrimaryScreenHeight;
                                        double screenWidth = SystemParameters.PrimaryScreenWidth;
                                        double xScale = screenWidth / 1920;
                                        double yScale = screenHeight / 1080;
                                        _popupDictionaryService.TextMenuOperationsPosition = new Point(boundingRect.X * xScale, boundingRect.Y * yScale + 20);
                                        _popupDictionaryService.TextMenuAPIPosition = new Point(boundingRect.X * xScale, boundingRect.Y * yScale + 60);
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
        ObserverSelectionChangeElement = automationElement;
        Automation.AddAutomationEventHandler(TextPattern.TextSelectionChangedEvent, ObserverSelectionChangeElement, TreeScope.Element, OnTextSelectionChange);
    }

    private string GetTextFromFocusedControl()
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
    }

    [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr WindowFromPoint(Point pt);

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    public static extern int SendMessageW([InAttribute] System.IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
    public const int WM_GETTEXT = 13;

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GetFocus();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowThreadProcessId(int handle, out int processId);

    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    internal static extern int AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
    [DllImport("kernel32.dll")]
    internal static extern int GetCurrentThreadId();

    private static AutomationElement FindFirstElementType(AutomationElement automationElement, ControlType controlType)
    {
        AutomationElement? element = null;
        try
        {
            IntPtr handle = NativeUser32API.GetForegroundWindow();
            AutomationElement rootElement = AutomationElement.FromHandle(handle);
            var elementColections = rootElement.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, controlType));
            Debug.WriteLine("======================================================================================");
            Debug.WriteLine("ROOT ELEMENT: " + rootElement.Current.Name);
            Debug.WriteLine("GROUP ELEMENT COUNT: " + elementColections.Count);
            if(elementColections.Count > 0)
            {
                foreach (AutomationElement groupElement in elementColections)
                {
                    AutomationElementCollection childs = groupElement.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition);
                    foreach (AutomationElement child in childs)
                    {
                        Debug.WriteLine("CHILD ELEMENT: " + child.Current.ControlType.ProgrammaticName);
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return element;
    }
}