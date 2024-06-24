using AB15_GUI.WPF.Models;
using AB15_GUI.WPF.Models.Interfaces;
using AB15_GUI.WPF.Services.Interfaces;
using AB15_GUI.WPF.Views;
using NLog;

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
