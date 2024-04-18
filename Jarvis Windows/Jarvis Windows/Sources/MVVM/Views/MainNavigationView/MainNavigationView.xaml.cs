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

namespace Jarvis_Windows.Sources.MVVM.Views.MainNavigationView
{
    /// <summary>
    /// Interaction logic for MainNavigationView.xaml
    /// </summary>
    public partial class MainNavigationView : Window
    {
        public MainNavigationView()
        {
            InitializeComponent();

            //Startup postion
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2;
        }
    }
}
