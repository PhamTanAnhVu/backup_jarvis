using Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;
using Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;
using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.MVVM.Views;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;
using Point = System.Drawing.Point;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using System.Windows.Controls.Primitives;
using Jarvis_Windows.Sources.MVVM.Views.InjectionAction;
using System.Windows.Data;
using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions;

namespace Jarvis_Windows.Sources.Utils.Services;

public class PopupDictionaryService : ObserveralObject
{
    private bool _isShowJarvisAction;
    private bool _isShowMenuOperations;
    private bool _jarvisActionVisibility;
    private bool _textMenuSelectionVisibility;
    private bool _isShowAIBubbleFromTrayMenu;
    private bool _isShowAIChatBubble; 
    private bool _isShowAIChatSidebar; 
    private bool _isPinMenuSelectionResponse;

    private Point _jarvisActionPosition;
    private Point _menuOperationsPosition;

    private Point _aIChatBubblePosition;
    private Point _aIChatSidebarPosition;
    private static String? _targetLanguage;
    private Point _automationElementVisualPos;
    private TextMenuViewModel? _textMenuViewModel = null;
    private InjectionActionViewModel _injectionActionViewModel;
    private MenuSelectionActionsViewModel _menuSelectionActionsViewModel;
    private MenuSelectionResponseViewModel _menuSelectionResponseViewModel;
    private MenuSelectionPopupListViewModel _menuSelectionPopupListViewModel;

    //Popups 
    private Popup? _injectionActionPopup;
    private MenuInjectionActionsView? _menuinjectionActionsView;
    private MenuInjectionActionsViewModel? _menuinjectionActionsViewModel;
    private Popup _injectionActionPopup;
    private Popup _menuSelectionActionsPopup;
    private Popup _menuSelectionResponsePopup;
    private Popup _menuSelectionPopupListPopup;
    
    public static  String TargetLangguage
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
    public bool IsShowAIChatSidebar
    {
        get { return _isShowAIChatSidebar; }
        set
        {
            _isShowAIChatSidebar = value;
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
    public Point AIChatBubblePosition
    {
        get { return _aIChatBubblePosition; }
        set
        {
            _aIChatBubblePosition = value;
            OnPropertyChanged();
        }
    }
    public Point AIChatSidebarPosition
    {
        get { return _aIChatSidebarPosition; }
        set
        {
            _aIChatSidebarPosition = value;
            OnPropertyChanged();
        }
    }
    public Point JarvisActionPosition
    {
        get { return _jarvisActionPosition; }
        set
        {
            _jarvisActionPosition = value;
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

    private bool _isShowMenuSelectionActions;
    private bool _isShowMenuSelectionResponse;
    private bool _isShowMenuSelectionPopupList;

    private Point _menuSelectionActionsPosition;
    private Point _menuSelectionResponsePosition;
    private Point _menuSelectionPopupListPosition;

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
    public static bool HasPinnedJarvisButton = true;

    public PopupDictionaryService()
    {
        IsShowJarvisAction = false;
        IsShowMenuOperations = false;
        IsShowAIChatBubble = true;
        IsShowAIChatSidebar = false;

        JarvisActionPosition = new Point(0, 0);
        MenuOperationsPosition = new Point(0, 0);
        AIChatBubblePosition = new Point((int)(SystemParameters.WorkArea.Right - 30), (int)(SystemParameters.WorkArea.Bottom - 30) / 2);
        AIChatSidebarPosition = new Point((int)(SystemParameters.WorkArea.Right - 520), (int)(SystemParameters.WorkArea.Bottom - 700) / 2);

        InitInjectionAction();
        InitMenuSelectionActions();
        InitMenuSelectionResponse();
        InitMenuSelectionPopupList();
    }
    public void ShowJarvisAction(bool isShow)
    {
        IsShowJarvisAction = isShow;
        //IsShowJarvisAction = isShow & JarvisActionVisibility;
    }
    private void InitInjectionAction()
    {
        _injectionActionPopup = new Popup();
        InjectionActionView injectionActionView = new InjectionActionView();
        _injectionActionViewModel = (InjectionActionViewModel)injectionActionView.DataContext;
        _injectionActionPopup.SetCurrentValue(Popup.ChildProperty, injectionActionView);
        _injectionActionPopup.SetCurrentValue(Popup.AllowsTransparencyProperty, true);
        _injectionActionPopup.SetCurrentValue(Popup.PlacementProperty, PlacementMode.AbsolutePoint);
        _injectionActionPopup.SetCurrentValue(Popup.StaysOpenProperty, true);
        _injectionActionPopup.SetCurrentValue(UIElement.IsEnabledProperty, true);

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

        //Init menu injection actions
        //_menuinjectionActionsView = new MenuInjectionActionsView();
        //_menuinjectionActionsViewModel = (MenuInjectionActionsViewModel)_menuinjectionActionsView.DataContext;
    }

    private void InitMenuSelectionActions()
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

        IsShowMenuSelectionActions = true;
        Binding isOpenBinding = new Binding("IsShowMenuSelectionActions");
        isOpenBinding.Source = this;
        isOpenBinding.NotifyOnSourceUpdated = true;
        _menuSelectionActionsPopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
    }
    private void InitMenuSelectionResponse()
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

        IsShowMenuSelectionResponse = true;
        Binding isOpenBinding = new Binding("IsShowMenuSelectionResponse");
        isOpenBinding.Source = this;
        isOpenBinding.NotifyOnSourceUpdated = true;
        _menuSelectionResponsePopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
    }
 
    private void InitMenuSelectionPopupList()
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

        IsShowMenuSelectionPopupList = true;
        Binding isOpenBinding = new Binding("IsShowMenuSelectionPopupList");
        isOpenBinding.Source = this;
        isOpenBinding.NotifyOnSourceUpdated = true;
        _menuSelectionPopupListPopup.SetBinding(Popup.IsOpenProperty, isOpenBinding);
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
        if(systemPoint.X == 0 || systemPoint.Y == 0)
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
    }

    public void ShowMenuOperations(bool isShow)
    {
        //IsShowMenuOperations = isShow;
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

    public void ShowMenuSelectionActions(bool isShow)
    {
        IsShowMenuSelectionActions = isShow & TextMenuSelectionVisibility;
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

    public void ShowAIChatBubble(bool isShow)
    {
        IsShowAIChatBubble = isShow;
    }

    public void UpdateAIChatBubblePosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        AIChatBubblePosition = visualPoint;
    }
    
    public void ShowAIChatSidebar(bool isShow)
    {
        IsShowAIChatSidebar = isShow;
    }

    public void UpdateAIChatSidebarPosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        AIChatSidebarPosition = visualPoint;
    }

    internal void ShowSelectionResponseView(bool bIsShow)
    {
        IsShowMenuSelectionResponse = bIsShow;
    }

    public int GetMenuSelectionActionWidth()
    {
        if(_textMenuViewModel == null)
            _textMenuViewModel = DependencyInjection.GetService<TextMenuViewModel>();

        return _textMenuViewModel.GetMenuSelectionActionWidth();
    }

    public int GetMenuSelectionActionHeight()
    {
        if (_textMenuViewModel == null)
            _textMenuViewModel = DependencyInjection.GetService<TextMenuViewModel>();

        return _textMenuViewModel.GetMenuSelectionActionHeight();
    }
}
