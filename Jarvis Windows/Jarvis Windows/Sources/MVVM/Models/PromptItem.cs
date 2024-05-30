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
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int NumberParam { get; set; }
        public List<string>? ListParam { get; set; }

        public PromptItem()
        {
            Title = "";
            Content = "";
            NumberParam = 0;
            ListParam = new List<string>();
        }

        public PromptItem(string title, string content, int numberParam, List<string> listParam)
        {
            Title = title;
            Content = content;
            NumberParam = numberParam;
            ListParam = listParam;
        }
    }
}
