using Jarvis_Windows.Sources.Utils.Core;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class SettingButtonModel : ViewModelBase
{
    private int _idx;
    private string _labelName;
    private string _background;
    private string _foreground;
    private string _fontWeight;
    private RelayCommand _command;
    public int Idx 
    { 
        get { return _idx; }
        set
        {
            _idx = value;
            OnPropertyChanged();
        }
    }
    public string LabelName
    { 
        get { return _labelName; }
        set
        {
            _labelName = value;
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
    public string Foreground
    { 
        get { return _foreground; }
        set
        {
            _foreground = value;
            OnPropertyChanged();
        }
    }
    
    public string FontWeight
    { 
        get { return _fontWeight; }
        set
        {
            _fontWeight = value;
            OnPropertyChanged();
        }
    }
    public RelayCommand Command
    { 
        get { return _command; }
        set
        {
            _command = value;
            OnPropertyChanged();
        }
    }
}

