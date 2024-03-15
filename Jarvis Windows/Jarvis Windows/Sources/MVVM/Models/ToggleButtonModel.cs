using Jarvis_Windows.Sources.Utils.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class Size
{
    public double Width { get; set; }
    public double Height { get; set; }
    public Size(double width, double height)
    {
        Width = width;
        Height = height;
    }
}

public class ToggleButtons : ViewModelBase
{
    private bool _isOnline;
    private string _dotMargin;
    private string _barBackground;
    public int Idx { get; set; }
    public string Description { get; set; }
    public int DotSpeed { get; set; }
    public Size DotSize { get; set; }
    public Size GridSize { get; set; }
    public RelayCommand ToggleCommand { get; set; }
    public string DotMargin
    {
        get { return _dotMargin; }
        set
        {
            _dotMargin = value;
            OnPropertyChanged();
        }

    }
    public bool IsOnline
    {
        get { return _isOnline; }
        set
        {
            _isOnline = value;
            OnPropertyChanged();
        }
    }

    public string BarBackground
    {
        get { return _barBackground; }
        set
        {
            _barBackground = value;
            OnPropertyChanged();
        }
    }
}

public class ToggleButtonTemplate
{
    public ObservableCollection<ToggleButtons> ToggleButtonList { get; set; } = new ObservableCollection<ToggleButtons>
    {
        new ToggleButtons
        {
            Description = "Display Jarvis action in your textbox",
            IsOnline = true,
            DotSpeed = 7,
            DotSize = new Size(20, 20),
            GridSize = new Size(50, 28),

        },
        new ToggleButtons
        {
            Description = "Display Jarvis selection menu when you select text",
            IsOnline = true,
            DotSpeed = 7,
            DotSize = new Size(20, 20),
            GridSize = new Size(50, 28)
        },
        new ToggleButtons
        {
            Description = "Display Jarvis AI Chat on your right sidebar",
            IsOnline = true,
            DotSpeed = 7,
            DotSize = new Size(20, 20),
            GridSize = new Size(50, 28)
        },
    };
}