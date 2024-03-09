using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.Utils.Services;

public class PopupDictionaryService : ObserveralObject
{
    private bool _isShowJarvisAction;
    private bool _isShowMenuOperations;
    private bool _isShowTextMenuOperations;
    private bool _isShowTextMenuAPI;

    private Point _jarvisActionPosition;
    private Point _menuOperationsPosition;    
    private Point _textMenuOperationsPosition;
    private Point _textMenuAPIPosition;

    private static String? _targetLanguage;
    private Point _automationElementVisualPos;

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

    public Point JarvisActionPosition
    {
        get { return _jarvisActionPosition; }
        set
        {
            _jarvisActionPosition = value;
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

    public MainView MainWindow { get; set; }
    public bool IsDragging { get; internal set; }
    public Point AutomationElementVisualPos { get => _automationElementVisualPos; set => _automationElementVisualPos = value; }
    public static bool HasPinnedJarvisButton = true;

    public PopupDictionaryService()
    {
        IsShowJarvisAction = false;
        IsShowMenuOperations = false;
        JarvisActionPosition = new Point(0, 0);
        MenuOperationsPosition = new Point(0, 0);
    }

    public void ShowJarvisAction(bool isShow)
    {
        IsShowJarvisAction = isShow;
    }

    private Point ConvertFromSystemCoorToVisualCoord(Point systemPoint)
    {
        Point visualPos = new Point();

        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
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
        }));

        return visualPos;
    }

    public void UpdateJarvisActionPosition(Point systemPoint, Rect elementRectBounding)
    {

        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double horizontalScale = screenWidth / 1920;
        double verticalScale = screenHeight / 1080;

        Point jarvisButtonPos = new Point(0, 0);
        jarvisButtonPos.X = systemPoint.X * horizontalScale;
        jarvisButtonPos.Y = (systemPoint.Y - elementRectBounding.Height * 2) * verticalScale;

        JarvisActionPosition = jarvisButtonPos;
        MenuOperationsPosition = jarvisButtonPos;
    }

    public void ShowMenuOperations(bool isShow)
    {
        IsShowMenuOperations = isShow;
    }

    public void ShowMenuSelectionActions(bool isShow)
    {
        IsShowTextMenuOperations = isShow;
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
        IsShowTextMenuAPI = isShow;
    }

    public void UpdateMenuOperationsPosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        MenuOperationsPosition = visualPoint;
    }
    public void UpdateTextMenuOperationsPosition(Point systemPoint)
    {
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        // TextMenuOperationsPosition = MenuOperationsPosition;
    }

    public void UpdateTextMenuAPIPosition()
    {
        Point visualPoint = TextMenuOperationsPosition;
        visualPoint.Y += 40;

        TextMenuAPIPosition = visualPoint;
    }

    internal void ShowSelectionResponseView(bool bIsShow)
    {
        IsShowTextMenuAPI = bIsShow;
    }
}
