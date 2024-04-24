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

    public static void PublishMenuSelectionCommandExecuted(object sender, EventArgs e)
    {
        MenuSelectionCommandExecuted?.Invoke(sender, e);
    }

    public static void PublishMenuSelectionPopupListExecuted(object sender, EventArgs e)
    {
        MenuSelectionPopupListExecuted?.Invoke(sender, e);
    }


}
