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

namespace AB15_GUI.WPF.Views;
/// <summary>
/// Interaction logic for LoggerWindow.xaml
/// </summary>
public partial class LoggerWindow : Window
{
    /// <summary>
    /// 
    /// </summary>
    private Image _imageBrush1;

    /// <summary>
    /// 
    /// </summary>
    private Image _imageBrush2;

    /// <summary>
    /// 
    /// </summary>
    private bool _isImage1Active;

    /// <summary>
    /// 
    /// </summary>
    public LoggerWindow()
    {
        InitializeComponent();
        _imageBrush1 = (Image)this.FindResource("AscendingSortIco");
        _imageBrush2 = (Image)this.FindResource("DescendingSortIco");
        _isImage1Active = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SortingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isImage1Active)
        {
            SortingButton.Content = _imageBrush2;
        }
        else
        {
            SortingButton.Content = _imageBrush1;
        }

        _isImage1Active = !_isImage1Active;
    }
}
