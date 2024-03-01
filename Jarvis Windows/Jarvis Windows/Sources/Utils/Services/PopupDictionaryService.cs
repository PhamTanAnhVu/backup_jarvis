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
    private Point _jarvisActionPosition;
    private Point _menuOperationsPosition;
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

        Point jarvisButtonPos = new Point(0, 0);
        jarvisButtonPos.X = systemPoint.X;
        jarvisButtonPos.Y = (systemPoint.Y < screenHeight / 2) ? systemPoint.Y - elementRectBounding.Height * 2: systemPoint.Y - elementRectBounding.Height * 2;

        JarvisActionPosition = jarvisButtonPos;
    }

    public void ShowMenuOperations(bool isShow)
    {
        IsShowMenuOperations = isShow;
    }

    public void UpdateMenuOperationsPosition(Point systemPoint, Rect elementRectBounding)
    {
        double screenHeight = SystemParameters.PrimaryScreenHeight;

        Point menuAiActionsPos = new Point(0, 0);
        menuAiActionsPos.X = systemPoint.X;
        menuAiActionsPos.Y = (systemPoint.Y < screenHeight / 2) ? systemPoint.Y - elementRectBounding.Height * 2 : systemPoint.Y - elementRectBounding.Height * 2;

        MenuOperationsPosition = menuAiActionsPos;
    }
}
