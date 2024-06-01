using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models
{
    public class PromptCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public PromptCategory()
        {
            Id = 0;
            Name = "";
        }

        public PromptCategory(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
