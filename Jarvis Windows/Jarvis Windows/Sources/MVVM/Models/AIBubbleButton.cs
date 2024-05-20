using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Jarvis_Windows.Sources.MVVM.Models;
public class AIBubbleButton : ViewModelBase
{
    private string? _color;
    public string Color
    {
        get { return _color; }
        set
        {
            _color = value;
            OnPropertyChanged();
        }
    }
}
