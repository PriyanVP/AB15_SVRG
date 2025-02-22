using Microsoft.Extensions.DependencyInjection;
using AB15_GUI.WPF;

namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// View model locator class. Provides ViewModels for views as bindable properties
    /// </summary>
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => App.AppHost.Services.GetRequiredService<MainViewModel>();
        public LoggerViewModel LoggerViewModel => App.AppHost.Services.GetRequiredService<LoggerViewModel>();
        public WatchdogViewModel WatchdogViewModel => App.AppHost.Services.GetRequiredService<WatchdogViewModel>();
        public FiringViewModel FiringViewModel => App.AppHost.Services.GetRequiredService<FiringViewModel>();
        public SPILearningViewModel SPILearningViewModel => App.AppHost.Services.GetRequiredService<SPILearningViewModel>();
        public ConfigurationViewModel ConfigurationViewModel => App.AppHost.Services.GetRequiredService<ConfigurationViewModel>();
    }
}