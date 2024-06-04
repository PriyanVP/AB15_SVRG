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
    public class MainViewModel : ViewModelBase
    {
        private readonly Logger logger;

        public readonly LoggerViewModel LoggerViewModel;


        public MainViewModel(Logger logger, LoggerViewModel loggerViewModel, LoggerView loggerWindowView)
        {
            this.logger = logger;
            LoggerViewModel = loggerViewModel;

            logger.Trace("In MainViewModel");
            loggerWindowView.Show();
        }
    }
}
