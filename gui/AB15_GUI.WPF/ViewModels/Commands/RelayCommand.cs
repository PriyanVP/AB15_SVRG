using System;
using System.Windows.Input;

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

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return (canExecute == null) ? (true) : (canExecute(parameter));
        }

        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
