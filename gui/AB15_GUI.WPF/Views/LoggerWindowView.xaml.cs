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

        DataContext = this;

        // Find image source and safe
        AscendingSortIco = (Image)this.FindResource("AscendingSortIco");
        DescendingSortIco = (Image)this.FindResource("DescendingSortIco");

        // set defoult flag
        _isImage1Active = true;

        // Create method for sorting
        //collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Items);
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

            // delete and create new sort descriptions
            //collectionView.SortDescriptions.Clear();
            //collectionView.SortDescriptions.Add(new SortDescription("LogNumber", ListSortDirection.Descending));
            _isImage1Active = false;
        }
        else
        {
            SortingButton.Content = AscendingSortIco;

            // delete and create new sort descriptions
            //collectionView.SortDescriptions.Clear();
            //collectionView.SortDescriptions.Add(new SortDescription("LogNumber", ListSortDirection.Ascending));
            _isImage1Active = true;
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
