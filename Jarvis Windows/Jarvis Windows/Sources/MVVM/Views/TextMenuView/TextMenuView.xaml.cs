using Gma.System.MouseKeyHook;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Jarvis_Windows.Sources.MVVM.Views.TextMenuView
{
    public partial class TextMenuView : UserControl
    {
        private IKeyboardMouseEvents globalMouseHook;
        public TextMenuView()
        {
            InitializeComponent();
        }

        private void TextMenuPopup_MouseEnter(object sender, EventArgs e)
        {
            EventAggregator.PublishMouseOverTextMenuPopupChanged(true, EventArgs.Empty);
        }

        private void TextMenuPopup_MouseLeave(object sender, EventArgs e)
        {
            EventAggregator.PublishMouseOverTextMenuPopupChanged(false, EventArgs.Empty);
        }


    }
}