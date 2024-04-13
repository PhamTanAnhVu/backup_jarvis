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

namespace Jarvis_Windows.Sources.Utils.Services;

public class PopupDictionaryService : ObserveralObject
{
    private bool _isShowJarvisAction;
    private bool _isShowMenuOperations;
    private bool _isShowTextMenuOperations;
    private bool _isShowTextMenuAPI;
    private bool _jarvisActionVisibility;
    private bool _textMenuSelectionVisibility;
    private bool _isShowAIBubbleFromTrayMenu;
    private bool _isShowAIChatBubble; 
    private bool _isShowAIChatSidebar; 
    private bool _isShowPinTextMenuAPI;
    private bool _isShowPopupTextMenu;

    private Point _jarvisActionPosition;
    private Point _menuOperationsPosition;    
    private Point _textMenuOperationsPosition;
    private Point _textMenuAPIPosition;
    private Point _popupTextMenuPosition;

    private Point _aIChatBubblePosition;
    private Point _aIChatSidebarPosition;
    private static String? _targetLanguage;
    private Point _automationElementVisualPos;
    private TextMenuViewModel? _textMenuViewModel = null;
    
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
            if (value == false && _isShowMenuOperations)
            {
                int n = 5;
            }
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
    public Point AIChatSidebarPosition
    {
        get { return _aIChatSidebarPosition; }
        set
        {
            _aIChatSidebarPosition = value;
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
    }
    public void ShowJarvisAction(bool isShow)
    {
        IsShowJarvisAction = isShow & JarvisActionVisibility;
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
        IsShowMenuOperations = isShow & JarvisActionVisibility;
    }

    public void ShowMenuSelectionActions(bool isShow)
    {
        IsShowTextMenuOperations = isShow & TextMenuSelectionVisibility;
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
        IsShowTextMenuAPI = isShow & TextMenuSelectionVisibility;
    }

    public void UpdateMenuOperationsPosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        MenuOperationsPosition = visualPoint;
    }
    public void UpdateTextMenuOperationsPosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
    }

    public void UpdateTextMenuAPIPosition()
    {
        Point visualPoint = TextMenuOperationsPosition;
        visualPoint.Y += 40;

        TextMenuAPIPosition = visualPoint;
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
        IsShowTextMenuAPI = bIsShow;
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
