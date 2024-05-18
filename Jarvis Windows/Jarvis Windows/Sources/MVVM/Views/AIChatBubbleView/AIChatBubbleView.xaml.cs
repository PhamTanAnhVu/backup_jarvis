using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.Utils.Services;
using System.IO;
using System;

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
    }

    private void Global_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (!_isInit || !PopupDictionaryService.Instance().IsShowAIChatBubble)
        {
            return;
        }

        Point mousePosition = new Point(e.X, e.Y);
        Point bubbleButtonPosition = BubbleButton.PointToScreen(new Point(0, 0));
        Point extraButtonPosition = ExtraButton.PointToScreen(new Point(0, 0));
        Point settingButtonPosition = SettingButton.PointToScreen(new Point(0, 0));

        double margin = 10;
        double marginTop = 10, marginBottom = 10 + BubbleButton.ActualHeight;
        
        if (_isShowExtraButtons)
        {
            if (mousePosition.Y >= bubbleButtonPosition.Y - margin - marginTop && 
                mousePosition.Y <= bubbleButtonPosition.Y + margin + marginBottom)
            {
                if (mousePosition.X >= bubbleButtonPosition.X)
                {
                    return;
                }
            }

            if (mousePosition.Y >= extraButtonPosition.Y - margin &&
                mousePosition.Y <= extraButtonPosition.Y + margin + ExtraButton.ActualHeight)
            {
                if (mousePosition.X >= extraButtonPosition.X - margin &&
                    mousePosition.X <= extraButtonPosition.X + margin + ExtraButton.Width)
                {
                    return;
                }
            }

            else if (mousePosition.Y >= settingButtonPosition.Y - margin && 
                    mousePosition.Y <= settingButtonPosition.Y + margin + SettingButton.ActualHeight)
            {
                if (mousePosition.X >= settingButtonPosition.X - margin && 
                    mousePosition.X <= settingButtonPosition.X + margin + SettingButton.ActualWidth)
                {
                    return;
                }
            }
        }

        _isShowExtraButtons = false;
        CloseBubblePopup.IsOpen = false;

        ExtraButton.Visibility = Visibility.Hidden;
        SettingButton.Visibility = Visibility.Hidden;

        BubbleButton.Width = 44;
        BubbleButtonName.Visibility = Visibility.Hidden;
    }

    private void MouseEnter_BubbleButton(object sender, MouseEventArgs e)
    {
        _isInit = true;
        if (_isShowExtraButtons) return;

        _isShowExtraButtons = true;
        CloseBubblePopup.IsOpen = true;

        ExtraButton.Visibility = Visibility.Visible;
        SettingButton.Visibility = Visibility.Visible;
        
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
