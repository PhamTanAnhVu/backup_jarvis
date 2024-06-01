using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Models
{
    public class PublicPromptItem
    {
        public int Index { get; set; }
        public PromptCategory? Category { get; set; }
        public string? Title { get; set; }
        public RelayCommand? DetailCommand { get; set; }
        public RelayCommand? AddFavoriteCommand { get; set; }
        public RelayCommand? ShowInfoCommand { get; set; }
        public bool? IsFavorite { get; set; }
    }
}
