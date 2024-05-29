using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public static class AIChatSidebarEventTrigger
{
    public static event EventHandler<EventArgs>? MouseOverInfoPopup;
    public static event EventHandler<EventArgs>? MouseOverHistoryPopup;
    public static event EventHandler<EventArgs>? ChatIdxChanged;
    public static event EventHandler<EventArgs>? SelectConversationChanged;
    public static event EventHandler<EventArgs>? ScrollChatToBottom;

    public static void PublishMouseOverInfoPopup(object sender, EventArgs e)
    {
        MouseOverInfoPopup?.Invoke(sender, e);
    }
    public static void PublishMouseOverHistoryPopup(object sender, EventArgs e)
    {
        MouseOverHistoryPopup?.Invoke(sender, e);
    }

    public static void PublishChatIdxChanged(object sender, EventArgs e)
    {
        ChatIdxChanged?.Invoke(sender, e);
    }

    public static void PublishSelectConversationChanged(object sender, EventArgs e)
    {
        SelectConversationChanged?.Invoke(sender, e);
    }

    public static void PublishScrollChatToBottom(object sender, EventArgs e)
    {
        ScrollChatToBottom?.Invoke(sender, e);
    }


}
