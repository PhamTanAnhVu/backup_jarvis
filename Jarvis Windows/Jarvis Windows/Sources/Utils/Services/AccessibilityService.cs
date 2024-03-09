using System;
using System.Windows.Automation;
using System.Windows;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Security.RightsManagement;
using System.Windows.Automation.Text;
using System.Xml.Linq;

namespace Jarvis_Windows.Sources.Utils.Services
{
    public class AccessibilityService_
    {
        private static AccessibilityService_? Instance;
        private static AutomationElement? _focusingElement;
        private static AutomationFocusChangedEventHandler? _focusChangedEventHandler = new AutomationFocusChangedEventHandler(OnElementFocusChanged);

        private static PopupDictionaryService? _popupDictionaryService;
        private static SendEventGA4? _sendEventGA4;
        private static bool _isUseAutoTuningPosition = true;
        private static IAutomationElementValueService? _automationElementValueService;
        public PopupDictionaryService? PopupDictionaryService
        {
            get { return _popupDictionaryService; }
            set => _popupDictionaryService = value;
        }

        public static bool IsUseAutoTuningPosition
        {
            get => _isUseAutoTuningPosition;
            set => _isUseAutoTuningPosition = value;
        }

        public AutomationFocusChangedEventHandler? FocusChangedEventHandler
        {
            get { return _focusChangedEventHandler; }
            set => _focusChangedEventHandler = value;
        }

        public static AccessibilityService_ GetInstance()
        {
            if (Instance == null)
                Instance = new AccessibilityService_();
            return Instance;
        }

        public SendEventGA4? SendEventGA4
        {
            get { return _sendEventGA4; }
            set => _sendEventGA4 = value;
        }

        public IAutomationElementValueService? AutomationElementValueService { get => _automationElementValueService; set => _automationElementValueService = value; }

        private AccessibilityService_(PopupDictionaryService popupDictionaryService)
        {
            PopupDictionaryService = popupDictionaryService;
        }

        public AccessibilityService_()
        {
        }

        public void SubscribeToElementFocusChanged()
        {
            Automation.AddAutomationFocusChangedEventHandler(_focusChangedEventHandler);
            Thread tuningJarivsPositionThread = new Thread(TunningPositionThread);
            tuningJarivsPositionThread.Name = "Jarvis Position Tuning";
            //tuningJarivsPositionThread.Start();
        }

        private void TunningPositionThread(object? obj)
        {
            if (_popupDictionaryService != null)
            {
                try
                {
                    while (true)
                    {
                        if (_focusingElement != null && IsUseAutoTuningPosition)
                        {
                            _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                            _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                        }
                        Thread.Sleep(33);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
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
                /*else if (automationElement.Current.ControlType.ProgrammaticName.Equals("ControlType.Document"))
                    return true;*/
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

            if(newFocusElement != null && _popupDictionaryService != null)
            {
                try
                {
                    /*TextPattern? textPattern = newFocusElement.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                    if (textPattern != null)
                    {

                        TextPatternRange[] selections = textPattern.GetSelection();

                        if (selections != null && selections.Length > 0)
                        {
                            TextPatternRange selection = selections[0];
                            Rect boundingRect = selection.GetBoundingRectangles()[0];

                            _popupDictionaryService.ShowJarvisAction(true);
                            _popupDictionaryService.ShowMenuOperations(false);
                            _popupDictionaryService.UpdateJarvisActionPosition(new Point(boundingRect.X, boundingRect.Y), boundingRect);
                            _popupDictionaryService.UpdateMenuOperationsPosition(new Point(boundingRect.X, boundingRect.Y), boundingRect);
                        }
                    }*/
                }
                catch (Exception)
                {
                    
                }
            }

            if (newFocusElement != null /*&& newFocusElement != _focusingElement*/)
            {
                if(_focusingElement != null)
                {
                    Automation.RemoveAutomationEventHandler(TextPattern.TextSelectionChangedEvent, _focusingElement, OnTextSelectionChange);
                    Automation.AddAutomationEventHandler(TextPattern.TextSelectionChangedEvent, _focusingElement, TreeScope.Element, OnTextSelectionChange);
                }

                if (IsEditableElement(newFocusElement))
                {
                    _focusingElement = newFocusElement;

                    

                    _popupDictionaryService.ShowJarvisAction(true);
                    _popupDictionaryService.ShowMenuOperations(false);
                    _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                    _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                    Task.Run(async () => await ExecuteSendEventInject());
                }
                else if(IsSuportedTextPattern(newFocusElement))
                {
                    /*TextPattern textPattern = newFocusElement.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                    if (textPattern != null)
                    {
                        TextPatternRange[] selections = textPattern.GetSelection();
                        if (selections != null && selections.Length > 0)
                        {
                            TextPatternRange selection = selections[0];
                            Rect boundingRect = selection.GetBoundingRectangles()[0];
                            Debug.WriteLine("Tọa độ bắt đầu của đoạn text: X = " + boundingRect.X + ", Y = " + boundingRect.Y);
                        }
                    }*/
                    /*_popupDictionaryService.ShowJarvisAction(true);
                    _popupDictionaryService.ShowMenuOperations(false);
                    _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(newFocusElement));
                    _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(newFocusElement));*/
                }
                else
                {
                    try
                    {
                        nint currentAppHandle = NativeUser32API.GetForegroundWindow();
                        AutomationElement foregroundApp = AutomationElement.FromHandle(currentAppHandle);
                        if (foregroundApp != null)
                        {
                            if (foregroundApp.Current.Name.Equals("Jarvis MainView"))
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

        private static bool IsSuportedTextPattern(AutomationElement newFocusElement)
        {
            if (newFocusElement != null)
            {
                TextPattern? textPattern;
                if (newFocusElement.TryGetCurrentPattern(TextPattern.Pattern, out object textPatternObj))
                {
                    textPattern = textPatternObj as TextPattern;
                    if (textPattern != null)
                        return true;
                }
                else if (newFocusElement.TryGetCurrentPattern(ValuePattern.Pattern, out object valuePatternObj))
                {
                    var valuePattern = valuePatternObj as ValuePattern;
                    if(valuePattern != null)
                        return true;
                }
            }
            return false;
        }

        private static void OnTextSelectionChange(object sender, AutomationEventArgs e)
        {
            GetSelectedText(_focusingElement);
        }

        private static Rect GetElementRectBounding(AutomationElement? automationElement)
        {
            if (automationElement == null)
            {
                return new Rect(0, 0, 0, 0);
            }

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
                    if (elementRectBounding.X < 0 || elementRectBounding.Y < 0)
                    {
                        _popupDictionaryService.ShowJarvisAction(false);
                        _popupDictionaryService.ShowMenuOperations(false);
                        placementPoint.X = 0;
                        placementPoint.Y = 0;
                    }
                    else
                    {
                        placementPoint.X = elementRectBounding.Left/* + elementRectBounding.Width*/;
                        placementPoint.Y = elementRectBounding.Top/* + elementRectBounding.Height * 0.5*/;
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
            catch (Exception)
            {
                throw;
            }
        }

        private void OnElementPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            var automationElement = sender as AutomationElement;
            if (e.Property == AutomationElement.BoundingRectangleProperty)
            {
                _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
                _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
            }
            else if (e.Property == AutomationElement.IsOffscreenProperty)
            {
                _popupDictionaryService.ShowJarvisAction(false);
                _popupDictionaryService.ShowMenuOperations(false);
            }
        }

        private AutomationElement FindChildEditElement(AutomationElement parrentElement)
        {
            System.Windows.Automation.Condition editCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit);

            AutomationElement editElement = parrentElement.FindFirst(TreeScope.Descendants, editCondition);

            return editElement;
        }

        public void SetValueForFocusingEditElement(string? value)
        {
            int timeoutMilliseconds = 200;
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
                            valuePattern.SetValue(value);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });

                if (!setValueTask.Wait(timeoutMilliseconds, cancellationToken))
                {
                    cancellationTokenSource.Cancel();
                    throw new TimeoutException("The SetValueForFocusingEditElement operation has timed out.");
                }
            }
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
                    catch (Exception)
                    {
                        throw;
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

        public static String GetSelectedText(AutomationElement? automationElement)
        {
           String selectedText = String.Empty;
            if(automationElement != null)
            {
                try
                {
                    TextPattern? textPattern;
                    if(automationElement.TryGetCurrentPattern(TextPattern.Pattern, out object textPatternObj))
                    {
                        textPattern = textPatternObj as TextPattern;
                        if (textPattern != null)
                        {
                            if (textPattern.SupportedTextSelection == SupportedTextSelection.None)
                            {
                                Debug.WriteLine("Selected text is not supported for this element.");
                            }
                            else
                            {
                                TextPatternRange[] selectedRanges = textPattern.GetSelection();
                                if (selectedRanges != null && selectedRanges.Length > 0)
                                {

                                    TextPatternRange selection = selectedRanges[0];
                                    if(selection != null)
                                    {
                                        var rects = selection.GetBoundingRectangles();
                                        if(rects != null && rects.Length > 0)
                                        {
                                            Rect boundingRect = rects[0];

                                            _popupDictionaryService.ShowMenuSelectionActions(true);
                                            _popupDictionaryService.ShowSelectionResponseView(false);
                                            double screenHeight = SystemParameters.PrimaryScreenHeight;
                                            double screenWidth = SystemParameters.PrimaryScreenWidth;
                                            double xScale = screenWidth / 1920;
                                            double yScale = screenHeight / 1080;
                                            _popupDictionaryService.TextMenuOperationsPosition = new Point(boundingRect.X * xScale, boundingRect.Y * yScale + 20);
                                            _popupDictionaryService.TextMenuAPIPosition = new Point(boundingRect.X * xScale, boundingRect.Y * yScale + 20);

                                            /*//selectedText = selectedRanges[0].GetText(1);
                                            selectedRanges[0].MoveEndpointByUnit(TextPatternRangeEndpoint.Start, TextUnit.Document, 1);
                                            selectedText = selectedRanges[0].GetText(-1);
                                            Debug.WriteLine("Selected text: " + selectedText);*/
                                        }

                                        //selection.GetText(-1);

                                    }
                                }
                                else
                                {
                                    selectedText = string.Empty;
                                    Debug.WriteLine("No text selected.");
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine("TextPattern is not supported for this element.");
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return selectedText;
        }

        internal AutomationElement? GetFocusingElement()
        {
            return _focusingElement;
        }

        private void RegisterSelectionChangedFor(AutomationElement automationElement)
        {
            Automation.RemoveAutomationEventHandler(TextPattern.TextSelectionChangedEvent, automationElement, OnTextSelectionChange);
            Automation.AddAutomationEventHandler(TextPattern.TextSelectionChangedEvent, automationElement, TreeScope.Element, OnTextSelectionChange);
        }
    }
}

