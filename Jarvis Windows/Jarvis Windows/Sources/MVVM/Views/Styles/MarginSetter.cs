using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Jarvis_Windows.Sources.MVVM.Views.Styles;

public class MarginSetter
{
    public static Thickness GetMargin(DependencyObject obj) => (Thickness)obj.GetValue(MarginProperty);

    public static void SetMargin(DependencyObject obj, Thickness value) => obj.SetValue(MarginProperty, value);

    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.RegisterAttached(nameof(FrameworkElement.Margin), typeof(Thickness),
            typeof(MarginSetter), new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

    private static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!(sender is Panel panel))
            return;

        panel.Loaded += Panel_Loaded;
    }

    private static void Panel_Loaded(object sender, RoutedEventArgs e)
    {
        if (!(sender is Panel panel))
            return;

        // Go over the children and set margin for them, except for the last child:
        for (int i = 0; i < panel.Children.Count - 1; i++)
        {
            if (panel.Children[i] is FrameworkElement fe)
            {
                fe.Margin = GetMargin(panel);
            }
        }
    }
}
