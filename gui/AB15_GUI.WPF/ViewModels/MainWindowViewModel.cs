using AB15_GUI.WPF.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB15_GUI.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Logger logger;

        public readonly LoggerViewModel LoggerViewModel;


        public MainWindowViewModel(Logger logger, LoggerViewModel loggerViewModel, LoggerWindowView loggerWindowView)
        {
            this.logger = logger;
            LoggerViewModel = loggerViewModel;

            logger.Trace("In MainWindowViewModel");
            loggerWindowView.Show();
        }
    }
}
