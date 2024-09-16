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
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// Empty + unused, required by ICommand interface
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add {}
            remove {}
        }

        /// <summary>
        /// Method to check if command can be executed
        /// </summary>
        /// <returns>flag indicating if command can be executed</returns>
        public bool CanExecute(object? parameter)
        {
            return (canExecute == null) ? (true) : (canExecute(parameter));
        }

        /// <summary>
        /// Command to be executed
        /// </summary>
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
