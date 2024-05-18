using Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;
using Jarvis_Windows.Sources.MVVM.Views.MainNavigationView;
using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.MVVM.Views.InjectionAction;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions;
using Jarvis_Windows.Sources.Utils.Core;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Point = System.Drawing.Point;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Jarvis_Windows.Sources.Utils.Services;

public class PopupDictionaryService : ObserveralObject
{
    private bool _isShowJarvisAction;
    private bool _isShowMenuOperations;
    private bool _jarvisActionVisibility;
    private bool _textMenuSelectionVisibility;
    private bool _isShowAIBubbleFromTrayMenu;
    private bool _isShowAIChatBubble; 
    private bool _isShowMainNavigation; 
    private bool _isPinMenuSelectionResponse;
    private bool _isShowPinTextMenuAPI;
    private bool _isShowPopupTextMenu;
    private bool _isShowTextMenuOperations;
    private bool _isShowTextMenuAPI;
    private bool _isShowMenuSelectionActions;
    private bool _isShowMenuSelectionResponse;
    private bool _isShowMenuSelectionPopupList;
    private DispatcherTimer _timer;
    private const int AutoCloseTimeInSeconds = 30;

    private Point _jarvisActionPosition;
    private Point _menuOperationsPosition;
    private Point _textMenuOperationsPosition;
    private Point _textMenuAPIPosition;
    private Point _popupTextMenuPosition;
    private Point _menuSelectionActionsPosition;
    private Point _menuSelectionResponsePosition;
    private Point _menuSelectionPopupListPosition;

    private Point _aIChatBubblePosition;
    private static String? _targetLanguage;
    private Point _automationElementVisualPos;
    private TextMenuViewModel? _textMenuViewModel = null;
    private InjectionActionViewModel _injectionActionViewModel;
    private MenuSelectionActionsViewModel _menuSelectionActionsViewModel;
    private MenuSelectionResponseViewModel _menuSelectionResponseViewModel;
    private MenuSelectionPopupListViewModel _menuSelectionPopupListViewModel;
    private System.Windows.Point _jarvisButtonPoint;

    //Popups 
    private Popup? _injectionActionPopup;
    private Popup? _menuinjectionActionsPopup;
    private MenuInjectionActionsViewModel? _menuinjectionActionsViewModel;
    private MenuInjectionActionsViewModel? _menuOperatorsViewModel;
    private static PopupDictionaryService? _instance = null;


    private Popup _menuSelectionActionsPopup;
    private Popup _menuSelectionResponsePopup;
    private Popup _menuSelectionPopupListPopup;

    public static String TargetLangguage
    {
        get { return _targetLanguage; }
        set
        {
            _targetLanguage = value;
        }
    }

    public bool IsShowJarvisAction
    {
        get { return _isShowJarvisAction; }
        set
        {
            _isShowJarvisAction = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowMenuOperations
    {
        get { return _isShowMenuOperations; }
        set
        {
            _isShowMenuOperations = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowAIChatBubble
    {
        get { return _isShowAIChatBubble; }
        set
        {
            _isShowAIChatBubble = value;
            OnPropertyChanged();
        }
    }
    public bool IsShowMainNavigation
    {
        get { return _isShowMainNavigation; }
        set
        {
            _isShowMainNavigation = value;
            OnPropertyChanged();
        }
    }
    public bool IsPinMenuSelectionResponse
    {
        get { return _isPinMenuSelectionResponse; }
        set
        {
            _isPinMenuSelectionResponse = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowTextMenuOperations
    {
        get { return _isShowTextMenuOperations; }
        set
        {
            _isShowTextMenuOperations = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowTextMenuAPI
    {
        get { return _isShowTextMenuAPI; }
        set
        {
            _isShowTextMenuAPI = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowPinTextMenuAPI
    {
        get { return _isShowPinTextMenuAPI; }
        set
        {
            _isShowPinTextMenuAPI = value;
            OnPropertyChanged();
        }
    }
    public Point AIChatBubblePosition
    {
        get { return _aIChatBubblePosition; }
        set
        {
            _aIChatBubblePosition = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowPopupTextMenu
    {
        get { return _isShowPopupTextMenu; }
        set
        {
            _isShowPopupTextMenu = value;
            OnPropertyChanged();
        }
    }

    public Point JarvisActionPosition
    {
        get { return _jarvisActionPosition; }
        set
        {
            _jarvisActionPosition = value;
            if (_injectionActionViewModel != null)
            {
                _injectionActionViewModel.HorizontalOffset = value.X;
                _injectionActionViewModel.VerticalOffset = value.Y;
            }
            OnPropertyChanged();
        }
    }
    public bool JarvisActionVisibility
    {
        get { return _jarvisActionVisibility; }
        set
        {
            _jarvisActionVisibility = value;
            OnPropertyChanged();
        }
    }
    public bool TextMenuSelectionVisibility
    {
        get { return _textMenuSelectionVisibility; }
        set
        {
            _textMenuSelectionVisibility = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowAIBubbleFromTrayMenu
    {
        get { return _isShowAIBubbleFromTrayMenu; }
        set
        {
            _isShowAIBubbleFromTrayMenu = value;
            OnPropertyChanged();
        }
    }

    public Point MenuOperationsPosition
    {
        get { return _menuOperationsPosition; }
        set
        {
            _menuOperationsPosition = value;
            OnPropertyChanged();
        }
    }

    public Point TextMenuOperationsPosition
    {
        get { return _textMenuOperationsPosition; }
        set
        {
            _textMenuOperationsPosition = value;
            OnPropertyChanged();
        }
    }
    public Point TextMenuAPIPosition
    {
        get { return _textMenuAPIPosition; }
        set
        {
            _textMenuAPIPosition = value;
            OnPropertyChanged();
        }
    }
    public Point PopupTextMenuPosition
    {
        get { return _popupTextMenuPosition; }
        set
        {
            _popupTextMenuPosition = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowMenuSelectionActions
    {
        get { return _isShowMenuSelectionActions; }
        set
        {
            _isShowMenuSelectionActions = value;
            OnPropertyChanged();
        }
    }

    public Point MenuSelectionActionsPosition
    {
        get { return _menuSelectionActionsPosition; }
        set
        {
            _menuSelectionActionsPosition = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowMenuSelectionResponse
    {
        get { return _isShowMenuSelectionResponse; }
        set
        {
            _isShowMenuSelectionResponse = value;
            OnPropertyChanged();
        }
    }

    public Point MenuSelectionResponsePosition
    {
        get { return _menuSelectionResponsePosition; }
        set
        {
            _menuSelectionResponsePosition = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowMenuSelectionPopupList
    {
        get { return _isShowMenuSelectionPopupList; }
        set
        {
            _isShowMenuSelectionPopupList = value;
            OnPropertyChanged();
        }
    }

    public Point MenuSelectionPopupListPosition
    {
        get { return _menuSelectionPopupListPosition; }
        set
        {
            _menuSelectionPopupListPosition = value;
            OnPropertyChanged();
        }
    }

    public MainView MainWindow { get; set; }
    public bool IsDragging { get; internal set; }
    public Point AutomationElementVisualPos { get => _automationElementVisualPos; set => _automationElementVisualPos = value; }
    public static PopupDictionaryService Instance()
    {
        if (_instance == null)
        {
            _instance = new PopupDictionaryService();
        }
        return _instance;
    }

    public static bool HasPinnedJarvisButton = true;

    private PopupDictionaryService()
    {
        _isShowJarvisAction = false;
        _isShowMenuOperations = false;
        _isShowAIChatBubble = false;

        _jarvisActionPosition = new Point(0, 0);
        _menuOperationsPosition = new Point(0, 0);

        _aIChatBubblePosition = new Point((int)(SystemParameters.WorkArea.Right), (int)(SystemParameters.WorkArea.Bottom) / 2);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(AutoCloseTimeInSeconds)
        };
        _timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (IsShowJarvisAction)
        {
            var storyboard = new Storyboard();
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            Storyboard.SetTarget(fadeOutAnimation, _injectionActionPopup?.Child);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(fadeOutAnimation);
            storyboard.Completed += (s, a) => {
                IsShowJarvisAction = false;
                _timer.Stop();
            };
            storyboard.Begin();
        }
    }

    public void ShowJarvisAction(bool isShow)
    {
        IsShowJarvisAction = isShow;
        if(isShow)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => //Access to the UI thread
            {
                var storyboard = new Storyboard();
                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };

                Storyboard.SetTarget(fadeInAnimation, _injectionActionPopup?.Child);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
                storyboard.Children.Add(fadeInAnimation);
                storyboard.Completed += (s, a) => {
                    IsShowJarvisAction = true;
                    _timer.Start();
                };
                storyboard.Begin();
            }));
        }
    }
    private void JarvisButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        IsDragging = false;
        _jarvisButtonPoint = e.GetPosition(null);
        _timer.Stop();
    }

    private void JarvisButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            System.Windows.Point currentMousePosition = e.GetPosition(null);
            double distance = (currentMousePosition - _jarvisButtonPoint).Length;

            if (distance > 2)
            {
                HasPinnedJarvisButton = false;
                IsDragging = true;
                System.Windows.Point relative = e.GetPosition(null);
                System.Windows.Point AbsolutePos = new System.Windows.Point(relative.X + _injectionActionPopup.HorizontalOffset, relative.Y + _injectionActionPopup.VerticalOffset);
                _injectionActionPopup.VerticalOffset = AbsolutePos.Y - _jarvisButtonPoint.Y;
                _injectionActionPopup.HorizontalOffset = AbsolutePos.X - _jarvisButtonPoint.X;

                _injectionActionPopup.VerticalOffset = AbsolutePos.Y - _jarvisButtonPoint.Y;
                _injectionActionPopup.HorizontalOffset = AbsolutePos.X - _jarvisButtonPoint.X;
            }
        }
    }

    private void JarviseButton_MouseLeave(object sender, MouseEventArgs e)
    {
        _timer.Start();
    }

    public void InitInjectionAction()
    {
        _injectionActionPopup = new DragMoveablePopup();
        InjectionActionView injectionActionView = new InjectionActionView();
        _injectionActionViewModel = (InjectionActionViewModel)injectionActionView.DataContext;
        _injectionActionPopup.SetCurrentValue(Popup.ChildProperty, injectionActionView);
        _injectionActionPopup.SetCurrentValue(Popup.AllowsTransparencyProperty, true);
        _injectionActionPopup.SetCurrentValue(Popup.PlacementProperty, PlacementMode.AbsolutePoint);
        _injectionActionPopup.SetCurrentValue(Popup.StaysOpenProperty, true);
        _injectionActionPopup.SetCurrentValue(UIElement.IsEnabledProperty, true);
        _injectionActionPopup.PreviewMouseLeftButtonDown += JarvisButton_PreviewMouseLeftButtonDown;
        _injectionActionPopup.PreviewMouseMove += JarvisButton_MouseMove;
        _injectionActionPopup.MouseLeave += JarviseButton_MouseLeave;

        Binding verticalBinding = new Binding("JarvisActionPosition.Y");
        verticalBinding.NotifyOnSourceUpdated = true;
        verticalBinding.Source = this;
        _injectionActionPopup.SetBinding(Popup.VerticalOffsetProperty, verticalBinding);

        Binding horizontalBinding = new Binding("JarvisActionPosition.X");
        horizontalBinding.NotifyOnSourceUpdated = true;
        horizontalBinding.Source = this;
        _injectionActionPopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBinding);

        Binding isOpenBinding = new Binding("IsShowJarvisAction");
        isOpenBinding.Source = this;
        isOpenBinding.NotifyOnSourceUpdated = true;
        _injectionActionPopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
    }

    public void InitMenuSelectionActions()
    {
        _menuSelectionActionsPopup = new Popup();
        MenuSelectionActionsView menuSelectionActionsView = new MenuSelectionActionsView();
        _menuSelectionActionsViewModel = (MenuSelectionActionsViewModel)menuSelectionActionsView.DataContext;
        _menuSelectionActionsPopup.SetCurrentValue(Popup.ChildProperty, menuSelectionActionsView);
        _menuSelectionActionsPopup.SetCurrentValue(Popup.AllowsTransparencyProperty, true);
        _menuSelectionActionsPopup.SetCurrentValue(Popup.PlacementProperty, PlacementMode.AbsolutePoint);
        _menuSelectionActionsPopup.SetCurrentValue(Popup.StaysOpenProperty, true);
        _menuSelectionActionsPopup.SetCurrentValue(UIElement.IsEnabledProperty, true);

        Binding verticalBinding = new Binding("MenuSelectionActionsPosition.Y");
        verticalBinding.NotifyOnSourceUpdated = true;
        verticalBinding.Source = this;
        _menuSelectionActionsPopup.SetBinding(Popup.VerticalOffsetProperty, verticalBinding);

        Binding horizontalBinding = new Binding("MenuSelectionActionsPosition.X");
        horizontalBinding.NotifyOnSourceUpdated = true;
        horizontalBinding.Source = this;
        _menuSelectionActionsPopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBinding);

        IsShowMenuSelectionActions = false;
        Binding isOpenBinding = new Binding("IsShowMenuSelectionActions");
        isOpenBinding.Source = this;
        isOpenBinding.NotifyOnSourceUpdated = true;
        _menuSelectionActionsPopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
    }
    public void InitMenuSelectionResponse()
    {
        _menuSelectionResponsePopup = new Popup();
        MenuSelectionResponseView menuSelectionResponseView = new MenuSelectionResponseView();
        _menuSelectionResponseViewModel = (MenuSelectionResponseViewModel)menuSelectionResponseView.DataContext;
        _menuSelectionResponsePopup.SetCurrentValue(Popup.ChildProperty, menuSelectionResponseView);
        _menuSelectionResponsePopup.SetCurrentValue(Popup.AllowsTransparencyProperty, true);
        _menuSelectionResponsePopup.SetCurrentValue(Popup.PlacementProperty, PlacementMode.AbsolutePoint);
        _menuSelectionResponsePopup.SetCurrentValue(Popup.StaysOpenProperty, true);
        _menuSelectionResponsePopup.SetCurrentValue(UIElement.IsEnabledProperty, true);

        MenuSelectionResponsePosition = new Point(100, 500);
        Binding verticalBinding = new Binding("MenuSelectionResponsePosition.Y");
        verticalBinding.NotifyOnSourceUpdated = true;
        verticalBinding.Source = this;
        _menuSelectionResponsePopup.SetBinding(Popup.VerticalOffsetProperty, verticalBinding);

        Binding horizontalBinding = new Binding("MenuSelectionResponsePosition.X");
        horizontalBinding.NotifyOnSourceUpdated = true;
        horizontalBinding.Source = this;
        _menuSelectionResponsePopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBinding);

        IsShowMenuSelectionResponse = false;
        Binding isOpenBinding = new Binding("IsShowMenuSelectionResponse");
        isOpenBinding.Source = this;
        isOpenBinding.NotifyOnSourceUpdated = true;
        _menuSelectionResponsePopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
    }

    public void InitMenuSelectionPopupList()
    {
        _menuSelectionPopupListPopup = new Popup();
        MenuSelectionPopupListView menuSelectionPopupListView = new MenuSelectionPopupListView();
        _menuSelectionPopupListViewModel = (MenuSelectionPopupListViewModel)menuSelectionPopupListView.DataContext;
        _menuSelectionPopupListPopup.SetCurrentValue(Popup.ChildProperty, menuSelectionPopupListView);
        _menuSelectionPopupListPopup.SetCurrentValue(Popup.AllowsTransparencyProperty, true);
        _menuSelectionPopupListPopup.SetCurrentValue(Popup.PlacementProperty, PlacementMode.AbsolutePoint);
        _menuSelectionPopupListPopup.SetCurrentValue(Popup.StaysOpenProperty, true);
        _menuSelectionPopupListPopup.SetCurrentValue(UIElement.IsEnabledProperty, true);

        MenuSelectionPopupListPosition = new Point(1000, 500);
        Binding verticalBinding = new Binding("MenuSelectionPopupListPosition.Y");
        verticalBinding.NotifyOnSourceUpdated = true;
        verticalBinding.Source = this;
        _menuSelectionPopupListPopup.SetBinding(Popup.VerticalOffsetProperty, verticalBinding);

        Binding horizontalBinding = new Binding("MenuSelectionPopupListPosition.X");
        horizontalBinding.NotifyOnSourceUpdated = true;
        horizontalBinding.Source = this;
        _menuSelectionPopupListPopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBinding);

        IsShowMenuSelectionPopupList = false;
        Binding isOpenBinding = new Binding("IsShowMenuSelectionPopupList");
        isOpenBinding.Source = this;
        isOpenBinding.NotifyOnSourceUpdated = true;
        _menuSelectionPopupListPopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
    }

    public void InitMenuInjectionActions()
    {
        _menuinjectionActionsPopup = new Popup();
        MenuInjectionActionsView menuOperatorsView = new MenuInjectionActionsView();
        _menuOperatorsViewModel = (MenuInjectionActionsViewModel)menuOperatorsView.DataContext;
        _menuinjectionActionsPopup.SetCurrentValue(Popup.ChildProperty, menuOperatorsView);
        _menuinjectionActionsPopup.SetCurrentValue(Popup.AllowsTransparencyProperty, true);
        _menuinjectionActionsPopup.SetCurrentValue(Popup.PlacementProperty, PlacementMode.AbsolutePoint);
        _menuinjectionActionsPopup.SetCurrentValue(Popup.StaysOpenProperty, true);
        _menuinjectionActionsPopup.SetCurrentValue(UIElement.IsEnabledProperty, true);

        Binding verticalBinding = new Binding("MenuOperationsPosition.Y");
        verticalBinding.NotifyOnSourceUpdated = true;
        verticalBinding.Source = this;
        _menuinjectionActionsPopup.SetBinding(Popup.VerticalOffsetProperty, verticalBinding);

        Binding horizontalBinding = new Binding("MenuOperationsPosition.X");
        horizontalBinding.NotifyOnSourceUpdated = true;
        horizontalBinding.Source = this;
        _menuinjectionActionsPopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBinding);

        IsShowMenuOperations = false;
        Binding isOpenBinding = new Binding("IsShowMenuOperations");
        isOpenBinding.NotifyOnSourceUpdated = true;
        isOpenBinding.Source = this;
        _menuinjectionActionsPopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
    }

    private Point ConvertFromSystemCoorToVisualCoord(Point systemPoint)
    {
        Point visualPos = new Point();

        /*Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (source != null)
            {
                visualPos = source.CompositionTarget.TransformFromDevice.Transform(new Point(systemPoint.X, systemPoint.Y));
                AutomationElementVisualPos = visualPos;
                visualPos.X = visualPos.X - 22;
                visualPos.Y = visualPos.Y - 22 / 2;

                int screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                Point jarvisButtonPos = new Point(0, 0);
                jarvisButtonPos.X = visualPos.X;
                jarvisButtonPos.Y = (visualPos.Y < screenHeight / 2) ? visualPos.Y + 30 : visualPos.Y - 30;
                JarvisActionPosition = jarvisButtonPos;


                if (visualPos.Y < 1080 / 2)
                {
                    visualPos.X = visualPos.X - 200;
                    visualPos.Y = visualPos.Y + 30;
                }
                else
                {
                    visualPos.X = visualPos.X - 200;
                    visualPos.Y = visualPos.Y - 180;

                }

                MenuOperationsPosition = jarvisButtonPos;
            }    
        }));*/

        return visualPos;
    }

    public void UpdateJarvisActionPosition(Point systemPoint, Rect elementRectBounding)
    {
        if (systemPoint.X == 0 || systemPoint.Y == 0)
            return;

        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double horizontalScale = screenWidth / 1920;
        double verticalScale = screenHeight / 1080;

        Point jarvisButtonPos = new Point(0, 0);
        jarvisButtonPos.X = (int)(systemPoint.X * horizontalScale);
        jarvisButtonPos.Y = (int)(systemPoint.Y * verticalScale - 30);

        JarvisActionPosition = jarvisButtonPos;
        MenuOperationsPosition = jarvisButtonPos;
        //if(_menuinjectionActionsViewModel != null)
            //_menuinjectionActionsViewModel.PositionChanged(jarvisButtonPos.Y, jarvisButtonPos.X);
    }

    public void ShowMenuOperations(bool isShow)
    {
        IsShowMenuOperations = isShow;
        //IsShowMenuOperations = isShow & JarvisActionVisibility;

        /*if (isShow)
        {
            _injectionActionPopup.IsOpen = false;
            _menuinjectionActionsView.Show();
        }
        else
        {
            _menuinjectionActionsView.Hide();
        }*/
    }

    public void UpdateMenuOperationsPosition(Point systemPoint, Rect elementRectBounding)
    {
        /*double screenHeight = SystemParameters.PrimaryScreenHeight;

        Point menuAiActionsPos = new Point(0, 0);
        menuAiActionsPos.X = systemPoint.X * xScale;
        menuAiActionsPos.Y = systemPoint.Y * yScale;

        MenuOperationsPosition = menuAiActionsPos;*/
    }

    public void ShowTextMenuAPIOperations(bool isShow)
    {
        IsShowMenuSelectionResponse = isShow & TextMenuSelectionVisibility;
    }

    public void UpdateMenuOperationsPosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        MenuOperationsPosition = visualPoint;
    }
    public void UpdateMenuSelectionActionsPosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
    }

    public void ShowMenuSelectionActions(bool isShow)
    {
        IsShowMenuSelectionActions = isShow;
        //IsShowMenuSelectionActions = isShow & TextMenuSelectionVisibility;
    }

    public void ShowAIChatBubble(bool isShow)
    {
        IsShowAIChatBubble = isShow;
    }

    public void UpdateAIChatBubblePosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        AIChatBubblePosition = visualPoint;
    }


    //public void UpdateAIChatSidebarPosition(Point systemPoint)
    //{
    //    Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
    //    AIChatSidebarPosition = visualPoint;
    //}

    internal void ShowSelectionResponseView(bool bIsShow)
    {
        IsShowMenuSelectionResponse = bIsShow;
    }

    public int GetMenuSelectionActionWidth()
    {
        if (_textMenuViewModel == null)
            _textMenuViewModel = DependencyInjection.GetService<TextMenuViewModel>();

        return _textMenuViewModel.GetMenuSelectionActionWidth();
    }

    public int GetMenuSelectionActionHeight()
    {
        if (_textMenuViewModel == null)
            _textMenuViewModel = DependencyInjection.GetService<TextMenuViewModel>();

        return _textMenuViewModel.GetMenuSelectionActionHeight();
    }

    public void PinJarvisButton()
    {
        _injectionActionPopup.HorizontalOffset = JarvisActionPosition.X;
        _injectionActionPopup.VerticalOffset = JarvisActionPosition.Y;

        //reset the position binding
        Binding verticalBinding = new Binding("JarvisActionPosition.Y");
        verticalBinding.NotifyOnSourceUpdated = true;
        verticalBinding.Source = this;
        _injectionActionPopup.SetBinding(Popup.VerticalOffsetProperty, verticalBinding);

        Binding horizontalBinding = new Binding("JarvisActionPosition.X");
        horizontalBinding.NotifyOnSourceUpdated = true;
        horizontalBinding.Source = this;
        _injectionActionPopup.SetBinding(Popup.HorizontalOffsetProperty, horizontalBinding);
    }
}
