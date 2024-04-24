using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models;

public class AIChatMessage
{
    public bool IsUser { get; set; }
    public bool IsJarvis { get; set; }
    public string? Message { get; set; }
    public bool IsLoading { get; set; }
    public bool IsBorderVisible { get; set; }
}
