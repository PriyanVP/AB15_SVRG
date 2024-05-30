using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

public class Person
{
    public DateTime Time { get; set; }
    public string LogLevel{ get; set; }
    public int LogNumber{ get; set; }
    public string Message{ get; set; }
}

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

    private bool _dateFormat;

    private static readonly Regex _regex = new Regex("[^0-9]+");

    private ObservableCollection<Person> _items;

    public ObservableCollection<Person> Items
    {
        get { return _items; } 
        set 
        {
            _items = value;
            OnPropertyChanged("Items");
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public LoggerWindow()
    {
        _dateFormat = true;
        InitializeComponent();
        DataContext = this;
        _imageBrush1 = (Image)this.FindResource("AscendingSortIco");
        _imageBrush2 = (Image)this.FindResource("DescendingSortIco");
        _isImage1Active = true;
        List<Person> people = new List<Person>();
        Items = new ObservableCollection<Person>()
        {
            new Person {Time = new DateTime(2023, 10, 4, 11, 55, 41, 769), LogLevel = "Debug", LogNumber = 30, Message = "XML regmap loaded"},
            new Person {Time = new DateTime(2023, 10, 4, 11, 55, 42, 769), LogLevel = "Debug", LogNumber = 30, Message = "XML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loaded"},
            new Person {Time = new DateTime(2023, 10, 4, 11, 55, 43, 769), LogLevel = "Debug", LogNumber = 30, Message = "XML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedvXML regmap loaded"}
        };

        AdjustSizeLastColumn();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        AdjustSizeLastColumn();
    }

    private void AdjustSizeLastColumn()
    {
        double newSize = LogTable.ActualWidth - (TimeColumn.ActualWidth + LogLevelColumn.ActualWidth + NumberColumn.ActualWidth + SystemParameters.VerticalScrollBarWidth);
        if (newSize < 0) { return; }
        MessageColumn.Width = newSize;
    }

    private void RecordFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RecordFormat.SelectedIndex == 0)
        {
            var newBinding = new Binding("Time")
            {
                StringFormat = "yyyy-MM-ddTHH:mm:ss.fff"
            };
            TimeColumn.DisplayMemberBinding = newBinding;
        }
        else
        {
            var newBinding = new Binding("Time")
            {
                StringFormat = "HH:mm:ss"
            };
            TimeColumn.DisplayMemberBinding = newBinding;
        }
        TimeColumn.Width = TimeColumn.ActualWidth;
        TimeColumn.Width = double.NaN;
    }

    /// <summary>
    /// Check of input text valid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FontSizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (IsTextAllowed(e.Text))
        {
            e.Handled = true;
        }
        else
        {
            e.Handled = false;
        }
    }

    /// <summary>
    /// Check if pasted text valid (CTRL+V)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FontSizeTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (IsTextAllowed((string)e.DataObject.GetData(typeof(string))))
        {
            e.CancelCommand();
        }
    }

    /// <summary>
    /// Check if input string contain ONLY numbers
    /// </summary>
    /// <param name="text">String to check</param>
    /// <returns>True if contain</returns>
    private static bool IsTextAllowed(string text)
    {
        return _regex.IsMatch(text);
    }
}
