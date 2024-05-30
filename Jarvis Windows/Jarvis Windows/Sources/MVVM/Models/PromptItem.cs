using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models
{
    public class PromptItem
    {
        public int Index { get; set; }
        public string? Title { get; set; }
        public RelayCommand? DetailCommand { get; set; }
        public RelayCommand? EditCommand { get; set; }
        public RelayCommand? DeleteCommand { get; set; }
    }
}
