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
    /// Interaction logic for testAnimationRev2.xaml
    /// </summary>
    public partial class testAnimationRev2 : UserControl
    {

        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public Brush SparkColor
        {
            get { return (Brush)GetValue(SparkColorProperty); }
            set { SetValue(SparkColorProperty, value); }
        }

        public testAnimationRev2()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register(
            "BorderColor", typeof(Brush), typeof(testAnimationRev2), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty SparkColorProperty = DependencyProperty.Register(
            "SparkColor", typeof(Brush), typeof(testAnimationRev2), new PropertyMetadata(Brushes.Red));
    }
}
