﻿using System;
using System.Windows.Automation;
using System.Windows;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace Jarvis_Windows.Sources.Utils.Accessibility;

public class UIElementDetector
{
    private static UIElementDetector? Instance;
    private static AutomationElement? _focusingElement;
    private static AutomationFocusChangedEventHandler? _focusChangedEventHandler = new AutomationFocusChangedEventHandler(OnElementFocusChanged);

    private static PopupDictionaryService? _popupDictionaryService;
    private static SendEventGA4? _sendEventGA4;
    private static bool _isUseAutoTuningPosition = true;
    public PopupDictionaryService? PopupDictionaryService
    {
        get { return _popupDictionaryService; }
        set => _popupDictionaryService = value;
    }

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
                    Thread.Sleep(33);
                }
            }
            catch (ElementNotAvailableException)
            {
            }
            catch (NullReferenceException)
            {
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

        if (newFocusElement != null && newFocusElement != _focusingElement)
        {
            if (newFocusElement.Current.AutomationId.Equals("Jarvis_Custom_Action_TextBox") ||
                newFocusElement.Current.ControlType.ProgrammaticName.Equals("ControlType.Window") ||
                newFocusElement.Current.ClassName.Equals(String.Empty))
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
                    }
                    _focusingElement = null;
                    _popupDictionaryService.ShowJarvisAction(false);
                    _popupDictionaryService.ShowMenuOperations(false);
                }
                catch (ArgumentException)
                {
                    Debug.WriteLine($"❌❌❌ Argument Exception");
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
            _popupDictionaryService.ShowJarvisAction(false);
            _popupDictionaryService.ShowMenuOperations(false);
        }
        catch (ElementNotAvailableException)
        {
            Debug.WriteLine($"Element is not available");
            _popupDictionaryService.ShowJarvisAction(false);
            _popupDictionaryService.ShowMenuOperations(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
            _popupDictionaryService.ShowJarvisAction(false);
            _popupDictionaryService.ShowMenuOperations(false);
        }
    }

    private void OnElementPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
    {
        var automationElement = sender as AutomationElement;
        if (e.Property == AutomationElement.BoundingRectangleProperty)
        {
            Debug.WriteLine($"🟧🟧🟧 {automationElement?.Current.Name} Bounding Rectangle Changed");
            _popupDictionaryService.UpdateJarvisActionPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
            _popupDictionaryService.UpdateMenuOperationsPosition(CalculateElementLocation(), GetElementRectBounding(_focusingElement));
        }
        else if (e.Property == AutomationElement.IsOffscreenProperty)
        {
            Debug.WriteLine($"👁️👁️👁️ {automationElement?.Current.ControlType.ProgrammaticName} Offscreen Property Changed");
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

    public void SetValueForFocusingEditElement(String? value)
    {
        int timeoutMilliseconds = 200;
        if (_focusingElement != null)
        {

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task setValueTask = Task.Run(() =>
            {
                Debug.WriteLine($"❌❌❌ Set Value of {_focusingElement.Current.ClassName} {_focusingElement.Current.ControlType.ProgrammaticName}");
                try
                {
                    ValuePattern? valuePattern = _focusingElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    if (valuePattern != null)
                        valuePattern.SetValue(value);
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
}