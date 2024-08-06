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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AB15_GUI.WPF.Views.Components
{
    /// <summary>
    /// Interaction logic for WatchdogSlider.xaml
    /// </summary>
    public partial class WatchdogSlider : UserControl
    {
        public WatchdogSlider()
        {
            InitializeComponent();
        }

        // TODO: why here? not used
        private void Slider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

    }
}
