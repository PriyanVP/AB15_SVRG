using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views;

/// <summary>
/// Interaction logic for LoggerWindow.xaml
/// </summary>
public partial class LoggerWindow : Window, INotifyPropertyChanged
{
    /// <summary>
    /// Image source for ascending sort ico
    /// </summary>
    private Image _ascendingSortIco;

    /// <summary>
    /// Image source for descending sort ico
    /// </summary>
    private Image _descendingSortIco;

    /// <summary>
    /// Flag to tell in which sort mode table set now
    /// </summary>
    private bool _isDescendingOrder = true;

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

    /// <summary>
    /// Constructor for LoggerView
    /// </summary>
    public LoggerWindow()
    {
        InitializeComponent();

        // Find image source and safe
        _ascendingSortIco = (Image)this.FindResource("AscendingSortIco");
        _descendingSortIco = (Image)this.FindResource("DescendingSortIco");

        // Create method for sorting
        //collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Items);
    }

    /// <summary>
    /// Method to fire PropertyChanged event for UI binding
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
        if (_isDescendingOrder)
        {
            SortingButton.Content = _descendingSortIco;

            // delete and create new sort descriptions
            //collectionView.SortDescriptions.Clear();
            //collectionView.SortDescriptions.Add(new SortDescription("LogNumber", ListSortDirection.Descending));
            _isDescendingOrder = false;
        }
        else
        {
            SortingButton.Content = _ascendingSortIco;

            // delete and create new sort descriptions
            //collectionView.SortDescriptions.Clear();
            //collectionView.SortDescriptions.Add(new SortDescription("LogNumber", ListSortDirection.Ascending));
            _isDescendingOrder = true;
        }
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
