using Gma.System.MouseKeyHook;
using System.Windows.Controls;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.MVVM.Views.AIChatSidebarView
{
    public class TransparentPanel : Panel
    {
        private IKeyboardMouseEvents globalMouseHook;
        private bool IsMouseOver_AIChatSidebar_Grid;
        private bool IsMouseOver_AIChatSidebar_InputTextbox;

        public TransparentPanel()
        {
            Background = Brushes.Transparent;
            IsHitTestVisible = false;

            globalMouseHook = Hook.GlobalEvents();
            globalMouseHook.MouseWheelExt += GlobalMouseHook_MouseWheelExt;

            EventAggregator.MouseOverAIChatPanelChanged += (sender, e) => {
                IsMouseOver_AIChatSidebar_Grid = (bool)sender;
            };
            
            EventAggregator.MouseOverAIChatInputTextboxChanged += (sender, e) => {
                IsMouseOver_AIChatSidebar_InputTextbox = (bool)sender;
            };
        }

        private void GlobalMouseHook_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            if (!IsMouseOver_AIChatSidebar_Grid || IsMouseOver_AIChatSidebar_InputTextbox) return; 
            var parentScrollViewer = FindParentScrollViewer(this);
            if (parentScrollViewer != null)
            {
                parentScrollViewer.ScrollToVerticalOffset(parentScrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private ScrollViewer FindParentScrollViewer(Visual visual)
        {
            while (visual != null)
            {
                if (visual is ScrollViewer scrollViewer)
                {
                    return scrollViewer;
                }
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return null;
        }
    }
}
