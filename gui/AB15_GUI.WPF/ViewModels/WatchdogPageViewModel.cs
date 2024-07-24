using AB15_GUI.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace AB15_GUI.WPF.ViewModels
{
    public class WatchdogPageViewModel : ViewModelBase
    {
        private int wd1ResponseTime;
        public int WD1ResponseTime
        {
            get => wd1ResponseTime;
            set
            {
                wd1ResponseTime = value;
                OnPropertyChanged();
            }
        }

        private int wd1LockTime;
        public int WD1LockTime
        {
            get => wd1LockTime;
            set
            {
                wd1LockTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Local logger instance
        /// </summary>
        private readonly Logger logger;

        public WatchdogPageViewModel(Logger logger)
        {
            this.logger = logger;
            logger.Trace("In WatchdogPageViewModel");

            WD1LockTime = 0;
            WD1ResponseTime = 0;
        }
    }
}
