using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class PromptModel : ViewModelBase
{
    public bool PromptType { get; set; }
    public string PromptName { get; set; }
    public string PromptContent { get; set; }
    public string PromptCategory { get; set; }
    public string PromptLanguage { get; set; }

    public RelayCommand MarkFavoriteCommand { get; set; }
    public RelayCommand DeletePromptCommand { get; set; }
    public RelayCommand ShowPromptDetailCommand { get; set; }
}
