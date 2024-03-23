using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EventAggregator
{
    public static event EventHandler<EventArgs> LanguageSelectionChanged;
    public static event EventHandler<EventArgs> AIChatBubbleStatusChanged;
    public static event EventHandler<EventArgs> MouseOverAIChatInputTextboxChanged;
    public static event EventHandler<EventArgs> MouseOverAppUIChanged;
    public static event EventHandler<EventArgs> MouseOverTextSelectionMenu;

    // Notify Language Changed to MainViewModel -> Execute TranslateCommand
    // Publish in MenuOperatorsView.xaml.cs
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
    
    public static void PublishMouseOverAppUIChanged(object sender, EventArgs e)
    {
        MouseOverAppUIChanged?.Invoke(sender, e);
    }

    public static void PublishMouseOverTextSelectionMenu(object sender, EventArgs e)
    {
        MouseOverTextSelectionMenu?.Invoke(sender, e);
    }
}
