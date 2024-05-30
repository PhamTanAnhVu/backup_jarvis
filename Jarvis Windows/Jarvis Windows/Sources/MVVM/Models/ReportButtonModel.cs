﻿using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class ReportButtonModel : ViewModelBase
{
    private string _background;
    public int Idx { get; set; }
    public string ReportName { get; set; }
    public string Background
    {
        get { return _background; }
        set
        {
            _background = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand SelectReportCommand { get; set; }
}
