using Jarvis_Windows.Sources.Utils.Services;
using Stfu.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.MVVM.Views.SettingView;

public partial class SettingView : Window
{
    public GoogleAnalyticService GoogleAnalyticService { get; internal set; }
    public PopupDictionaryService PopupDictionaryService { get; internal set; }
    public SettingView()
    {
        InitializeComponent();

        EventAggregator.SettingVisibilityChanged += (sender, e) =>
        {
            bool settingVisibility = (bool)sender;
            if (settingVisibility)
            {
                this.Show();
                WindowState = WindowState.Normal;
            }
        };
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void HideWindow_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseWindow_Click(object sender, RoutedEventArgs e)
    {
        this.Hide();
    }

    private void Setting_MouseEnter(object sender, EventArgs e)
    {
        //EventAggregator.PublishMouseOverAppUIChanged(true, EventArgs.Empty);
    }

    private void Setting_MouseLeave(object sender, EventArgs e)
    {
        //EventAggregator.PublishMouseOverAppUIChanged(false, EventArgs.Empty);
    }
}
