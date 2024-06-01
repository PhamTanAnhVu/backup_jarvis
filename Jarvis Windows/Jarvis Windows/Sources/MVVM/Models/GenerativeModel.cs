using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class GenerativeModel : ViewModelBase
{
    private string _background;
    public int Idx { get; set; }
    public string Margin { get; set; }
    public string ImageSource { get; set; }
    public string Name { get; set; }
    public string PopupDescription { get; set; }
    public string Tokens { get; set; }
    public string Background
    {
        get => _background;
        set
        {
            _background = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand SelectModelCommand { get; set; }
}
