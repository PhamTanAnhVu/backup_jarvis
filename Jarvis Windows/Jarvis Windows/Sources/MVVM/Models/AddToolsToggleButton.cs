using Jarvis_Windows.Sources.Utils.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class AddToolsToggleButton : ViewModelBase
{
    private bool _isActive;
    private bool _isOpenInfoPopup;
    private string _toggleDotMargin;
    private string _toggleBackground;
    public SvgData SvgData { get; set; }
    public int Idx { get; set; }
    public string Header { get; set; }
    public string InfoPopupDescription { get; set; }
    public string InfoPopupButtonName { get; set; }
    public double ToggleButtonWidth { get; set; }
    public double ToggleButtonHeight { get; set; }
    public double ToggleDotSize { get; set; }
    public RelayCommand ToggleCommand { get; set; }
    public RelayCommand InfoPopupCommand { get; set; }
    public string ToggleDotMargin
    {
        get { return _toggleDotMargin; }
        set
        {
            _toggleDotMargin = value;
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
    public bool IsOpenInfoPopup
    {
        get { return _isOpenInfoPopup; }
        set
        {
            _isOpenInfoPopup = value;
            OnPropertyChanged();
        }
    }

    public string ToggleBackground
    {
        get { return _toggleBackground; }
        set
        {
            _toggleBackground = value;
            OnPropertyChanged();
        }
    }
}
