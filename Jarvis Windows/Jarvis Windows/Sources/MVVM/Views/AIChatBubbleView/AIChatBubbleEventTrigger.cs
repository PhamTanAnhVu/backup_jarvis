using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;

public static class AIChatBubbleEventTrigger
{
    public static event EventHandler<EventArgs>? HoverExtraButtonEvent;

    public static void PublishHoverExtraButtonEvent(object sender, EventArgs e)
    {
        HoverExtraButtonEvent?.Invoke(sender, e);
    }
}
