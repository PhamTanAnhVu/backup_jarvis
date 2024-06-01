using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models
{
    public class PromptCategoryItem
    {
        public int Index { get; set; }
        public string? Name { get; set; }
        public RelayCommand? Command { get; set; }
    }
}
