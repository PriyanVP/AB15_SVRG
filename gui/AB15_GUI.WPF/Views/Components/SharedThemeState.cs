using System.Windows;

namespace AB15_GUI.WPF.Views.Components
{
    /// <summary>
    /// Dependency property for sharing theme property across all UI
    /// </summary>
    public class SharedThemeState : DependencyObject
    {
        public static readonly DependencyProperty IsDarkThemeProperty =
            DependencyProperty.Register("IsDarkTheme", typeof(bool), typeof(SharedThemeState), new PropertyMetadata(false));

        /// <summary>
        /// Property indicating if Dark theme was selected
        /// </summary>
        public bool IsDarkTheme
        {
            get { return (bool)GetValue(IsDarkThemeProperty); }
            set { SetValue(IsDarkThemeProperty, value); }
        }
    }
}
