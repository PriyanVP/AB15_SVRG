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
    /// Interaction logic for animatedDataFly.xaml
    /// </summary>
    public partial class animatedDataFly : UserControl
    {
        public Brush FillColor
        {
            get { return (Brush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public animatedDataFly()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register(
            "FillColor", typeof(Brush), typeof(animatedDataFly), new PropertyMetadata(Brushes.LightBlue));

        public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register(
            "ScaleX", typeof(double), typeof(animatedDataFly), new PropertyMetadata(1.0));

        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register(
            "ScaleY", typeof(double), typeof(animatedDataFly), new PropertyMetadata(1.0));

        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

    }
}
