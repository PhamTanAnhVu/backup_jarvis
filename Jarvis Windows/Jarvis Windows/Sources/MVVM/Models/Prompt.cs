using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models
{
    public class Prompt
    {
        public string? Name { get; set; }

        public PromptCategory Category { get; set; }

        public string? Discription { get; set; }

        public string? Content { get; set; }

        public List<string>? ListParam { get; set; }

        public string? OutputLanguage { get; set; }

        public Prompt()
        {
            Name = "";
            Category = new PromptCategory(0, "");
            Discription = "";
            Content = "";
            ListParam = new List<string>();
            OutputLanguage = "";
        }

        public Prompt(string name,PromptCategory category, string discription, string content, string outputLanguage)
        {
            Name = name;
            Category = category;
            Discription = discription;
            Content = content;
            ListParam = SeparateParam(content);
            OutputLanguage = outputLanguage;
        }

        private List<string>? SeparateParam(string content)
        {
            List<string> separateParam = new List<string>();
            int startIdx = content.IndexOf("[");
            int endIdx = content.IndexOf("]");
            for (int i = startIdx; i < content.Length; i++)
            {
                if (content[i] == '[')
                {
                    startIdx = i;
                }
                if (content[i] == ']')
                {
                    endIdx = i;
                    string param = content.Substring(startIdx + 1, endIdx - startIdx - 1);
                    separateParam.Add(param);
                }
            }
            return separateParam;
        }
    }
}
