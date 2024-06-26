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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AB15_GUI.WPF.Views.Components
{
    /// <summary>
    /// Animation control for data flow
    /// </summary>
    public partial class testAnimationRev2 : UserControl
    {
        /// <summary>
        /// Brush for data flow path borders 
        /// </summary>
        public Brush BorderColor
        {
            get { return (Brush)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        /// <summary>
        /// Background color for arrows
        /// </summary>
        public Brush FlyColor
        {
            get { return (Brush)GetValue(FlyColorProperty); }
            set { SetValue(FlyColorProperty, value); }
        }

        /// <summary>
        /// Builder
        /// </summary>
        public testAnimationRev2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add border color brush to control metadata
        /// </summary>
        public static readonly DependencyProperty BorderColorProperty = 
            DependencyProperty.Register("BorderColor", typeof(Brush), typeof(testAnimationRev2), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Add backgroud color brush to control metadata
        /// </summary>
        public static readonly DependencyProperty FlyColorProperty = 
            DependencyProperty.Register("FlyColor", typeof(Brush), typeof(testAnimationRev2), new PropertyMetadata(Brushes.Red));

        /// <summary>
        /// Add flag for toggle animation
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnabledProperty =
            DependencyProperty.Register("IsAnimationEnabled", typeof(bool), typeof(testAnimationRev2), new PropertyMetadata(default(bool), OnIsAnimationEnabledChanged));

        /// <summary>
        /// Get/set animation flag
        /// </summary>
        public bool IsAnimationEnabled
        {
            get => (bool)GetValue(IsAnimationEnabledProperty);
            set => SetValue(IsAnimationEnabledProperty, value);
        }

        /// <summary>
        /// Event that occure when flag change it state
        /// TODO: maybe move to XAML
        /// NOTE: Start and Stop functions cannot be moved 
        /// inside THIS function in corrent form
        /// </summary>
        private static void OnIsAnimationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is testAnimationRev2 control)
            {
                if ((bool)e.NewValue)
                {
                    control.StartAnimation();
                }
                else
                {
                    control.StopAnimation();
                }
            }
        }

        /// <summary>
        /// Start animation loop
        /// </summary>
        private void StartAnimation()
        {
            AnimationCanvas1.Visibility = Visibility.Visible;
            AnimationCanvas2.Visibility = Visibility.Visible;
            var storyboard = (Storyboard)Resources["TestAnimation1"];
            storyboard.Begin();
        }

        /// <summary>
        /// Stop animation loop
        /// </summary>
        private void StopAnimation()
        {
            AnimationCanvas1.Visibility = Visibility.Hidden;
            AnimationCanvas2.Visibility = Visibility.Hidden;
            var storyboard = (Storyboard)Resources["TestAnimation1"];
            storyboard.Stop();
        }
    }
}
