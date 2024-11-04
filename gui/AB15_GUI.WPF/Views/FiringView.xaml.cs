using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace AB15_GUI.WPF.Views
{
    /// <summary>
    /// Interaction logic for Firing.xaml
    /// </summary>
    public partial class Firing : Page
    {
        public ObservableCollection<TestInput> ConfigChannelsTable { get; set; }
        public ObservableCollection<TestOutput> FiringResultTable { get; set; }

        public Firing()
        {

            this.ConfigChannelsTable = new ObservableCollection<TestInput>();
            this.FiringResultTable = new ObservableCollection<TestOutput>();
            InitializeComponent();

            this.DataContext = this;

            this.ConfigChannelsTable.Add(new TestInput() { ASICID = 1, ChannelID = 1, IndexMode = 1 });
            this.ConfigChannelsTable.Add(new TestInput() { ASICID = 1, ChannelID = 2, IndexMode = 2 });
            this.ConfigChannelsTable.Add(new TestInput() { ASICID = 1, ChannelID = 3, IndexMode = 3 });
            this.ConfigChannelsTable.Add(new TestInput() { ASICID = 1, ChannelID = 4, IndexMode = 4 });

            this.FiringResultTable.Add(new TestOutput() { ASICID = 1, ChannelID = 1, ToFire = false, WasFired = true });
            this.FiringResultTable.Add(new TestOutput() { ASICID = 1, ChannelID = 2, ToFire = false, WasFired = true });
            this.FiringResultTable.Add(new TestOutput() { ASICID = 1, ChannelID = 3, ToFire = false, WasFired = true });
            this.FiringResultTable.Add(new TestOutput() { ASICID = 1, ChannelID = 4, ToFire = false, WasFired = true });


        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    public class TestInput
    {
        public int ASICID { get; set; }
        public int ChannelID { get; set; }
        public string IdentifierName { 
            get; 
            set; }
        public int _indexMode;
        public int IndexMode 
        { 
            get; 
            set; 
        }

        public TestInput()
        {
            IdentifierName = "None";
        }
    }

    public class TestOutput
    {
        public int ASICID { get; set; }
        public int ChannelID { get; set; }
        public string IdentifierName { get; set; }

        public bool ToFire { get; set; }
        public bool WasFired { get; set; }
        public TestOutput()
        {
            IdentifierName = "None";
        }
    }

}
