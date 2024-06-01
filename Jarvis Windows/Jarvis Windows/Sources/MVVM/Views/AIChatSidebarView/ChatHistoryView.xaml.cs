using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.Utils.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView;

public partial class ChatHistoryView : UserControl
{
    private SolidColorBrush startIconBrush;
    private SolidColorBrush trashIconBrush;
    private SolidColorBrush[] colors;

    private readonly IKeyboardMouseEvents _globalHook;
    private bool _mouseEnterHistoryPopup;
    public ChatHistoryView()
    {
        InitializeComponent();

        colors = new SolidColorBrush[]
        {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155")),
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FACC15")),
        };

        startIconBrush = trashIconBrush = colors[0];

        _globalHook = Hook.GlobalEvents();
        _globalHook.MouseClick += Global_MouseClick;
    }

    private void MouseEnter_FilterFavoriteButton(object sender, MouseEventArgs e)
    {
        if (StartIconSVG.Fill == colors[2]) return;
        startIconBrush = colors[1];
        StartIconSVG.SetCurrentValue(Shape.FillProperty, startIconBrush);
    }

    private void MouseLeave_FilterFavoriteButton(object sender, MouseEventArgs e)
    {
        if (StartIconSVG.Fill == colors[2]) return;
        startIconBrush = colors[0];
        StartIconSVG.SetCurrentValue(Shape.FillProperty, startIconBrush);
    }
    private void MouseClick_FilterFavoriteButton(object sender, RoutedEventArgs e)
    {
        if (StartIconSVG.Fill == colors[2])
        {
            Geometry pathData = Geometry.Parse("M3.22435 16.7071C3.13682 17.206 3.62909 17.5965 4.06383 17.3731L9.00171 14.8351L13.9396 17.3731C14.3743 17.5965 14.8666 17.206 14.7791 16.7071L13.8456 11.3864L17.8083 7.6102C18.1786 7.25729 17.9869 6.61204 17.4908 6.54156L11.98 5.7587L9.52285 0.891343C9.3015 0.452886 8.70191 0.452886 8.48057 0.891343L6.02344 5.7587L0.512652 6.54156C0.0165395 6.61204 -0.175161 7.25729 0.195168 7.6102L4.15779 11.3864L3.22435 16.7071ZM8.74203 13.5929L4.59556 15.7241L5.37659 11.2722C5.41341 11.0623 5.34418 10.8474 5.1935 10.7038L1.92331 7.58745L6.48215 6.93983C6.67061 6.91305 6.83516 6.79269 6.92406 6.61658L9.00171 2.50096L11.0794 6.61658C11.1683 6.79269 11.3328 6.91305 11.5213 6.93983L16.0801 7.58745L12.8099 10.7038C12.6592 10.8474 12.59 11.0623 12.6268 11.2722L13.4079 15.7241L9.26139 13.5929C9.0976 13.5088 8.90582 13.5088 8.74203 13.5929Z");
            StartIconSVG.SetCurrentValue(Path.DataProperty, pathData);
            StartIconSVG.SetCurrentValue(Shape.FillProperty, startIconBrush);
        }
        else
        {
            Geometry pathData = Geometry.Parse("M2.70924 11.582C2.41941 11.731 2.09124 11.4707 2.14959 11.1381L2.77188 7.59095L0.130132 5.07347C-0.116753 4.83819 0.0110467 4.40802 0.341789 4.36104L4.01565 3.83914L5.65374 0.594229C5.8013 0.301924 6.20102 0.301924 6.34858 0.594229L7.98667 3.83914L11.6605 4.36104C11.9913 4.40802 12.1191 4.83819 11.8722 5.07347L9.23044 7.59095L9.85273 11.1381C9.91108 11.4707 9.58291 11.731 9.29308 11.582L6.00116 9.89008L2.70924 11.582Z");
            StartIconSVG.SetCurrentValue(Path.DataProperty, pathData);
            StartIconSVG.SetCurrentValue(Shape.FillProperty, colors[2]);
        }
    }


    private void MouseEnter_ClearChatButton(object sender, MouseEventArgs e)
    {
        //if (TrashIconSVG.Fill == colors[2]) return;
        trashIconBrush = colors[1];
        TrashIconSVG.SetCurrentValue(Shape.FillProperty, trashIconBrush);
    }

    private void MouseLeave_ClearChatButton(object sender, MouseEventArgs e)
    {
        //if (TrashIconSVG.Fill == colors[2]) return;
        trashIconBrush = colors[0];
        TrashIconSVG.SetCurrentValue(Shape.FillProperty, trashIconBrush);
    }

    private void HistoryPopup_MouseEnter(object sender, MouseEventArgs e)
    {
        _mouseEnterHistoryPopup = true;
        AIChatSidebarEventTrigger.PublishMouseOverHistoryPopup(_mouseEnterHistoryPopup, EventArgs.Empty);
    }

    private void HistoryPopup_MouseLeave(object sender, MouseEventArgs e)
    {
        _mouseEnterHistoryPopup = false;
        AIChatSidebarEventTrigger.PublishMouseOverHistoryPopup(_mouseEnterHistoryPopup, EventArgs.Empty);
    }

    private void Global_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (_mouseEnterHistoryPopup) return;
        EditPopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false);
        DeletePopup.SetCurrentValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false);
    }

    private void ChatHistory_EditTitleTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        HwndSource source = (HwndSource)PresentationSource.FromVisual(textBox);
        if (source != null)
        {
            IntPtr handle = source.Handle;
            var currentAutomation = AutomationElement.FromHandle(handle);
            NativeUser32API.SetForegroundWindow(handle);
        }
    }
}