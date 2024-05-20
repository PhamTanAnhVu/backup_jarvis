using Jarvis_Windows.Sources.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EventAggregator
{
    public static event EventHandler<EventArgs>? LanguageSelectionChanged;
    public static event EventHandler<EventArgs>? AIChatBubbleStatusChanged;
    public static event EventHandler<EventArgs>? MouseOverAIChatInputTextboxChanged;
    public static event EventHandler<EventArgs>? MouseOverAIChatPanelChanged;
    public static event EventHandler<EventArgs>? SettingVisibilityChanged;
    public static event EventHandler<EventArgs>? LoginStatusChanged;
    public static event EventHandler<EventArgs>? ApiUsageChanged;
    
    public static event EventHandler<EventArgs>? PropertyMessageChanged;
    public static void PublishPropertyMessageChanged(PropertyMessage message, EventArgs? e)
    {
        PropertyMessageChanged?.Invoke(message, e);
    }

    public static void PublishLanguageSelectionChanged(object sender, EventArgs e)
    {
        LanguageSelectionChanged?.Invoke(sender, e);
    }
    
    public static void PublishAIChatBubbleStatusChanged(object sender, EventArgs e)
    {
        AIChatBubbleStatusChanged?.Invoke(sender, e);
    }
    
    public static void PublishMouseOverAIChatInputTextboxChanged(object sender, EventArgs e)
    {
        MouseOverAIChatInputTextboxChanged?.Invoke(sender, e);
    }
    
    public static void PublishSettingVisibilityChanged(object sender, EventArgs e)
    {
        SettingVisibilityChanged?.Invoke(sender, e);
    }
    
    public static void PublishLoginStatusChanged(object sender, EventArgs e)
    {
        LoginStatusChanged?.Invoke(sender, e);
    }

    public static void PublishApiUsageChanged(object sender, EventArgs e)
    {
        ApiUsageChanged?.Invoke(sender, e);
    }


}
