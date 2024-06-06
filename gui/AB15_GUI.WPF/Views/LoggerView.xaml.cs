using System;
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AB15_GUI.WPF.Views;

/// <summary>
/// Interaction logic for LoggerWindow.xaml
/// </summary>
public partial class LoggerView : Window, INotifyPropertyChanged
{
    /// <summary>
    /// Event to update binded properties in View
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

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
    /// Hndle to apply sorting for ListView
    /// </summary>
    private CollectionView _referenceForSorting;

    /// <summary>
    /// Constructor for LoggerView
    /// </summary>
    public LoggerView()
    {
        InitializeComponent();

        // Init reference for applying sorting
        _referenceForSorting = (CollectionView)CollectionViewSource.GetDefaultView(LogTable.Items);
        _referenceForSorting.SortDescriptions.Clear();
        _referenceForSorting.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
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
        if (SortingButton.IsChecked == false)
        {
            // delete and create new sort descriptions
            _referenceForSorting.SortDescriptions.Clear();
            _referenceForSorting.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
        }
        else
        {
            // delete and create new sort descriptions
            _referenceForSorting.SortDescriptions.Clear();
            _referenceForSorting.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Descending));
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

    public void ChangeTheme(Uri uri)
    {
        App.Current.Resources.Clear();
        App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ChangeTheme(new Uri("Views/Themes/Dark.xaml", UriKind.RelativeOrAbsolute));
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ChangeTheme(new Uri("Views/Themes/Light.xaml", UriKind.RelativeOrAbsolute));
    }
}
