using Jarvis_Windows.Sources.Utils.Core;
using Microsoft.Expression.Interactivity.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class PromptBracketItem : ViewModelBase
{
    private bool _isEmptyInputText;
    private string _inputText;
    public string Margin { get; set; }
    public string PreText { get; set; }
    public string InputText
    {
        get => _inputText;
        set
        {
            _inputText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEmptyInputText));

        }
    }

    public bool IsEmptyInputText
    {
        get
        {
            if (string.IsNullOrEmpty(_inputText)) _isEmptyInputText = true;
            else _isEmptyInputText = false;
            return _isEmptyInputText;
        }
        set
        { 
            _isEmptyInputText = value; 
            OnPropertyChanged();
        }
    }

    public double MinHeight { get; set; }
    public double MaxHeight { get; set; }
}
