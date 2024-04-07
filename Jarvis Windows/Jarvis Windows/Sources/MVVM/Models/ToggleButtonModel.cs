using Jarvis_Windows.Sources.Utils.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class ToggleButtonModel : ViewModelBase
{
    private bool _isActive;
    private string _dotMargin;
    private string _background;
    public int Idx { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public double ToggleButtonWidth { get; set; }
    public double ToggleButtonHeight { get; set; }
    public double DotSize { get; set; }
    public RelayCommand Command { get; set; }
    public string DotMargin
    {
        get { return _dotMargin; }
        set
        {
            _dotMargin = value;
            OnPropertyChanged();
        }

    }
    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            OnPropertyChanged();
        }
    }

    public string Background
    {
        get { return _background; }
        set
        {
            _background = value;
            OnPropertyChanged();
        }
    }
}
