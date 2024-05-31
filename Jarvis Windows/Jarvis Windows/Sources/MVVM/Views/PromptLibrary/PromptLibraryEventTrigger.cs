using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Views.PromptLibrary;

public static class PromptLibraryEventTrigger
{
    public static event EventHandler<EventArgs>? SaveEditChanged;

    public static void PublishSaveEditChanged(object sender, EventArgs e)
    {
        SaveEditChanged?.Invoke(sender, e);
    }
}