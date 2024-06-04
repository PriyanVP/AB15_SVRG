using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
public partial class LoggerWindow : Window, INotifyPropertyChanged
{
    /// <summary>
    /// Image source for ascending sort ico
    /// </summary>
    private Image AscendingSortIco;

    /// <summary>
    /// Image source for descending sort ico
    /// </summary>
    private Image DescendingSortIco;

    /// <summary>
    /// Flag to tell in which sort mode table set now
    /// </summary>
    private bool _isImage1Active;

    /// <summary>
    /// Used to sort ObservableCollection
    /// </summary>
    private CollectionView collectionView;

    /// <summary>
    /// Regex to validate enteret text in textBox
    /// </summary>
    private static readonly Regex _regex = new Regex(@"^[0-9]{1,2}$");

    /// <summary>
    /// Event to update changes in view
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Font size in log table
    /// </summary>
    private double _logTableFontSize = 20;

    /// <summary>
    /// Getter and setter for _logTableFontSize
    /// </summary>
    public double LogTableFontSize
    {
        get { return _logTableFontSize; }
        set 
        { 
            _logTableFontSize = value;
            OnPropertyChanged(); 
        }
    }

    // debug code
    private ObservableCollection<Person> _items;

    // debug code
    public ObservableCollection<Person> Items
    {
        get 
        { 
            return _items; 
        } 
        set 
        {
            _items = value;
            OnPropertyChanged("Items");
        }
    }

    /// <summary>
    /// Constructor for LoggerView
    /// </summary>
    public LoggerWindow()
    {
        InitializeComponent();

        DataContext = this;

        // Find image source and safe
        AscendingSortIco = (Image)this.FindResource("AscendingSortIco");
        DescendingSortIco = (Image)this.FindResource("DescendingSortIco");

        // set defoult flag
        _isImage1Active = true;

        // debug code
        Items = new ObservableCollection<Person>()
        {
            new Person {Time = new DateTime(2023, 10, 4, 11, 55, 41, 769), LogLevel = "Debug", LogNumber = 30, Message = "XML regmap loaded"},
            new Person {Time = new DateTime(2023, 10, 4, 11, 55, 42, 769), LogLevel = "Debug", LogNumber = 31, Message = "XML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedXML regmap loaded"},
            new Person {Time = new DateTime(2023, 10, 4, 11, 55, 43, 769), LogLevel = "Debug", LogNumber = 32, Message = "XML regmap loadedXML regmap loadedXML regmap loadedXML regmap loadedvXML regmap loaded"}
        };

        collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Items);
    }

    /// <summary>
    /// update on view updated value
    /// </summary>
    /// <param name="name">Name of Property</param>
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Change sorting rule for log table
    /// </summary>
    private void SortingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isImage1Active)
        {
            SortingButton.Content = DescendingSortIco;
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new SortDescription("LogNumber", ListSortDirection.Descending));
            _isImage1Active = false;
        }
        else
        {
            SortingButton.Content = AscendingSortIco;
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new SortDescription("LogNumber", ListSortDirection.Ascending));
            _isImage1Active = true;
        }
    }

    /// <summary>
    /// Check if input string contain ONLY numbers
    /// </summary>
    /// <param name="text">String to check</param>
    /// <returns>True if contain</returns>
    private static bool IsTextAllowed(string text)
    {
        return !_regex.IsMatch(text);
    }

    /// <summary>
    /// Increase Font size of log table
    /// </summary>
    private void IncreaseFont_Click(object sender, RoutedEventArgs e)
    {
        LogTableFontSize += 1;
    }

    /// <summary>
    /// Decrease Font size of log table
    /// </summary>
    private void DecreaseFont_Click(object sender, RoutedEventArgs e)
    {
        if (LogTableFontSize > 10)
        {
            LogTableFontSize -= 1;
        }
    }

}
