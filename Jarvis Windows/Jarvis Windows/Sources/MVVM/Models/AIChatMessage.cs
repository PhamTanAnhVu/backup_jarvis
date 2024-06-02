using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class AIChatMessage
{
    public int Idx { get; set; }
    public bool IsServer { get; set; }
    public bool IsUser { get; set; }
    public string Message { get; set; }
    public string SelectedModel { get; set; }
    public string SelectedModelName { get; set; }
    public string SelectedModelImageSource { get; set; }
    public int SelectedModelIdx { get; set; }
    public ObservableCollection<CodeMessage> DetailMessage { get; set; }
    public RelayCommand CopyCommand { get; set; }
    public RelayCommand RedoCommand { get; set; }
    public bool IsLoading { get; set; }
}
public class CodeMessage
{
    public int Idx { get; set; }
    public bool IsVisible { get; set; }
    public string TextContent { get; set; }
    public string Language { get; set; }
    public string CodeContent { get; set; }
}