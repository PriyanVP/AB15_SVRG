using NLog;
using System;
using System.Windows;
using System.Windows.Input;
using AB15_GUI.WPF.NLog;

namespace AB15_GUI.WPF.ViewModels.Commands
{
    /// <summary>
    /// Relay command implementation
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool>? canExecute;

        Logger logger;

        /// <summary>
        /// Creates new Relay Command
        /// </summary>
        /// <param name="execute">function that will be executed</param>
        /// <param name="canExecute">function that checks if execute can run. If not provided returns true</param>
        public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;

            this.logger = LogManager.Setup()
                    .SetupExtensions(ext => ext.RegisterLayoutRenderer<BuildConfigurationLayoutRenderer>("build-configuration"))
                    .SetupExtensions(ext => ext.RegisterTarget<LogMemoryRecordTarget>("MemoryRecord"))
                    .GetCurrentClassLogger(); // Same logger will be used across all tests
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add 
            { 
                if (canExecute != null)
                {
                   CommandManager.RequerySuggested += value;
                   logger.Debug("In add for CanExecuteChanged");
                }
            }
            remove 
            { 
                if (canExecute != null)
                {
                   CommandManager.RequerySuggested -= value;
                   logger.Debug("In remove for CanExecuteChanged");
                }
            }
        }

        // /// <summary>
        // /// Raises the <see cref="CanExecuteChanged" /> event.
        // /// </summary>
        // public static void RaiseCanExecuteChanged()
        // {
        //     //Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        // }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }

        public bool CanExecute(object? parameter)
        {
            logger.Debug($"Reevaluated CanExecute for {execute.Method.Name}");
            return (canExecute == null) ? (true) : (canExecute(parameter));
        }

        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
