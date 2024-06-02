using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.Utils.Services;
using System.IO;
using System;
using Stfu.Linq;
using Jarvis_Windows.Sources.MVVM.Models;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatBubbleView;
public partial class AIChatBubbleView : UserControl
{
    private bool _isInit;
    private bool _isShowExtraButtons;
    private readonly IKeyboardMouseEvents _globalHook;
    public AIChatBubbleView()
    {
        InitializeComponent();
        _globalHook = Hook.GlobalEvents();
        _globalHook.MouseMove += Global_MouseMove;
        CloseBubbleButton.Visibility = Visibility.Collapsed;
        BubbleButtonName.Visibility = Visibility.Collapsed;
        UpperExtraBubbleButonBorder.Visibility = Visibility.Hidden;
        LowerExtraBubbleButonBorder.Visibility = Visibility.Hidden;
    }

    private void Global_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (!_isInit || !PopupDictionaryService.Instance().IsShowAIChatBubble)
        {
            return;
        }

        Point mousePosition = new Point(e.X, e.Y);
        Point bubbleButtonPosition = BubbleButton.PointToScreen(new Point(0, 0));
        Point extraButtonPosition = UpperExtraBubbleButonBorder.PointToScreen(new Point(0, 0));
        Point settingButtonPosition = SettingButton.PointToScreen(new Point(0, 0));

        double scale = (extraButtonPosition.X > 3000) ? 2 : 1;

        double marginTop = extraButtonPosition.Y;
        double marginBottom = settingButtonPosition.Y + (SettingButton.ActualHeight) * scale;
        
        if (_isShowExtraButtons)
        {
            if (mousePosition.Y >= marginTop && mousePosition.Y <= marginBottom)
            {
                if (mousePosition.X >= bubbleButtonPosition.X)
                {
                    return;
                }
            }
        }

        _isShowExtraButtons = false;
        CloseBubbleButton.Visibility = Visibility.Collapsed;
        BubbleButton.Opacity = 0.5;
        UpperExtraBubbleButonBorder.Visibility = Visibility.Hidden;
        LowerExtraBubbleButonBorder.Visibility = Visibility.Hidden;

        BubbleButton.Width = 44;
        BubbleButtonName.Visibility = Visibility.Hidden;
    }

    private void MouseEnter_BubbleButton(object sender, MouseEventArgs e)
    {
        _isInit = true;
        if (_isShowExtraButtons) return;

        _isShowExtraButtons = true;
        BubbleButton.Opacity = 1;
        CloseBubbleButton.Visibility = Visibility.Visible;

        UpperExtraBubbleButonBorder.Visibility = Visibility.Visible;
        LowerExtraBubbleButonBorder.Visibility = Visibility.Visible;
        
        BubbleButton.Width = 102;
        BubbleButtonName.Visibility = Visibility.Visible;
    }

    private void MouseEnter_ExtraButton(object sender, MouseEventArgs e)
    {
        Button button = (Button)sender;
        string content = $"{button.CommandParameter} True";
        AIChatBubbleEventTrigger.PublishHoverExtraButtonEvent(content, e);
    }

    private void MouseLeave_ExtraButton(object sender, MouseEventArgs e)
    {
        Button button = (Button)sender;
        string content = $"{button.CommandParameter} False";
        AIChatBubbleEventTrigger.PublishHoverExtraButtonEvent(content, e);
    }
}
