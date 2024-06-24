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
    public partial class DataArrow : UserControl
    {
        /// <summary>
        /// Arrow Background color
        /// </summary>
        public Brush FillColor
        {
            get { return (Brush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        /// <summary>
        /// Scale to ritate vertically
        /// </summary>
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        /// <summary>
        /// Scale to rotate Horizontally
        /// </summary>
        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        /// <summary>
        /// Builder
        /// </summary>
        public DataArrow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add backcolor to control metadata
        /// </summary>
        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register(
            "FillColor", typeof(Brush), typeof(DataArrow), new PropertyMetadata(Brushes.LightBlue));

        /// <summary>
        /// Add vertical rotation to control metadata
        /// </summary>
        public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register(
            "ScaleX", typeof(double), typeof(DataArrow), new PropertyMetadata(1.0));

        /// <summary>
        /// Add horizontal rotation to control metadata
        /// </summary>
        public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register(
            "ScaleY", typeof(double), typeof(DataArrow), new PropertyMetadata(1.0));


    }
}
