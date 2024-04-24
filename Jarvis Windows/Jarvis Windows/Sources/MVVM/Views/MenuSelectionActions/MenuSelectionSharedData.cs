using Microsoft.Expression.Interactivity.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuSelectionActions;

public static class MenuSelectionSharedData
{
    public static event EventHandler<EventArgs>? MenuSelectionCommandExecuted;
    public static event EventHandler<EventArgs>? MenuSelectionPopupListExecuted;
    public static event EventHandler<EventArgs>? MenuSelectionPopupPinExecuted;
    public static event EventHandler<EventArgs>? MouseOverActions;
    public static event EventHandler<EventArgs>? MouseOverResponse;
    public static event EventHandler<EventArgs>? MouseOverPopup;

    public static void PublishMenuSelectionCommandExecuted(object sender, EventArgs e)
    {
        MenuSelectionCommandExecuted?.Invoke(sender, e);
    }

    public static void PublishMenuSelectionPopupListExecuted(object sender, EventArgs e)
    {
        MenuSelectionPopupListExecuted?.Invoke(sender, e);
    }

    public static void PublishMenuSelectionPopupPinExecuted(object sender, EventArgs e)
    {
        MenuSelectionPopupPinExecuted?.Invoke(sender, e);
    }

    public static void PublishMouseOverActions(object sender, EventArgs e)
    {
        MouseOverActions?.Invoke(sender, e);
    }

    public static void PublishMouseOverResponse(object sender, EventArgs e)
    {
        MouseOverResponse?.Invoke(sender, e);
    }

    public static void PublishMouseOverPopup(object sender, EventArgs e)
    {
        MouseOverPopup?.Invoke(sender, e);
    }


}
