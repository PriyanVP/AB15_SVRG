using System;
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Policy;
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
    /// Event to update bind properties in View
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Font size in log table
    /// </summary>
    private uint _logTableFontSize = 20;

    /// <summary>
    /// Getter and setter for _logTableFontSize
    /// </summary>
    public uint LogTableFontSize
    {
        get { return _logTableFontSize; }
        set 
        { 
            _logTableFontSize = value;
            OnPropertyChanged(); 
        }
    }

    /// <summary>
    /// Handle to apply sorting for ListView
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
        _referenceForSorting.SortDescriptions.Clear();
        if (SortingButton.IsChecked == false)
        {
            // delete and create new sort descriptions
            _referenceForSorting.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
        }
        else
        {
            // delete and create new sort descriptions
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

    /// <summary>
    /// Event to change themes (light/dark)
    /// </summary>
    /// <param name="sender">theme change switch</param>
    /// <param name="e">parameters</param>
    private void ThemeChangeToggle_Click(object sender, RoutedEventArgs e)
    {

        foreach (var dict in App.Current.Resources.MergedDictionaries) 
        {
            // Remove theme dictionary
            if ((dict.Source.OriginalString == "Views/Themes/Light.xaml") || (dict.Source.OriginalString == "Views/Themes/Dark.xaml"))
            {
                App.Current.Resources.MergedDictionaries.Remove(dict);
                break;
            }
        }


        if (ThemeChangeToggle.IsChecked == false)
        {
            // Light theme
            App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Views/Themes/Light.xaml", UriKind.RelativeOrAbsolute) });
        }
        else
        {
            // Dark theme
            App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Views/Themes/Dark.xaml", UriKind.RelativeOrAbsolute) });
        }
    }
}