using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Jarvis_Windows.Sources.MVVM.Models;
public class AIButton : ViewModelBase
{
    private bool _visibility;
    private bool _extraIconVisibility;
    private string _idx;
    private double _width;
    private double _separateLineWidth;
    private string? _content;
    private string? _icon;
    private string? _commandParameter;
    private string? _pinColor;
    private string? _horizontalAlignment;
    private string? _cornerRadius;
    private RelayCommand? _command;
    private RelayCommand? _pinCommand;
    private Thickness _margin;
    public string Idx
    {
        get { return _idx; }
        set
        {
            _idx = value;
            OnPropertyChanged();
        }
    }

    public string Content
    {
        get { return _content; }
        set
        {
            _content = value;
            OnPropertyChanged();
        }
    }

    public string Icon
    {
        get { return _icon; }
        set
        {
            _icon = value;
            OnPropertyChanged();
        }
    }

    public bool Visibility
    {
        get { return _visibility; }
        set
        {
            _visibility = value;
            OnPropertyChanged();
        }
    }

    public bool ExtraIconVisibility
    {
        get { return _extraIconVisibility; }
        set
        {
            _extraIconVisibility = value;
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

    public RelayCommand PinCommand
    {
        get { return _pinCommand; }
        set
        {
            _pinCommand = value;
            OnPropertyChanged();
        }
    }

    public string CommandParameter
    {
        get { return _commandParameter; }
        set
        {
            _commandParameter = value;
            OnPropertyChanged();
        }
    }
    public string PinColor
    {
        get { return _pinColor; }
        set
        {
            _pinColor = value;
            OnPropertyChanged();
        }
    }

    public double Width
    {
        get { return _width; }
        set
        {
            _width = value;
            OnPropertyChanged();
        }
    }

    public Thickness Margin
    {
        get { return _margin; }
        set
        {
            _margin = value;
            OnPropertyChanged();
        }
    }

    public string HorizontalAlignment
    {
        get { return _horizontalAlignment; }
        set
        {
            _horizontalAlignment = value;
            OnPropertyChanged();
        }
    }

    public string CornerRadius
    {
        get { return _cornerRadius; }
        set
        {
            _cornerRadius = value;
            OnPropertyChanged();
        }
    }

    public double SeparateLineWidth
    {
        get { return _separateLineWidth; }
        set
        {
            _separateLineWidth = value;
            OnPropertyChanged();
        }
    }
}
