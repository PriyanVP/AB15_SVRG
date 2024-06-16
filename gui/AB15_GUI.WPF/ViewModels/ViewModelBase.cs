using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace AB15_GUI.WPF.ViewModels
{
    /// <summary>
    /// Base class for all View Models. Implements OnPropertyChanged event and error provider
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Dictionary that stores list of errors for each property
        /// </summary>
        private readonly Dictionary<string, List<string>> _propertyNameToErrorsDictionary = new Dictionary<string, List<string>>();

        /// <summary>
        /// Property that shows if any error is present
        /// </summary>
        public bool HasErrors => (_propertyNameToErrorsDictionary.Count > 0);

        /// <summary>
        /// Event for notification if property has changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Event for notification if error state has changed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Virtual method to get errors for property.
        /// Can get property name by caller if propertyName is not specified
        /// </summary>
        /// <param name="propertyName">name of property for getting errors</param>
        /// <returns></returns>
        public virtual IEnumerable GetErrors([CallerMemberName] string? propertyName = null)
        {
            Contract.Requires<ArgumentNullException>((propertyName is not null), "Argument can't be null!");

            return _propertyNameToErrorsDictionary.GetValueOrDefault(propertyName, new List<string>());
        }

        /// <summary>
        /// Method to add errors to property
        /// </summary>
        /// <param name="errorMessage">error message</param>
        /// <param name="propertyName">name of property</param>
        private void AddError(string errorMessage, [CallerMemberName] string? propertyName = null)
        {
            Contract.Requires<ArgumentNullException>((propertyName is not null), "Argument can't be null!");

            // Create key and empty list for property if not yet present
            if (!_propertyNameToErrorsDictionary.ContainsKey(propertyName))
            {
                _propertyNameToErrorsDictionary.Add(propertyName, new List<string>());
            }

            // Add error message to property error list
            _propertyNameToErrorsDictionary[propertyName].Add(errorMessage);

            // Raise event to notify that error state has changed
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clear all errors for certain property
        /// </summary>
        /// <param name="propertyName">name of property</param>
        private void ClearErrors([CallerMemberName] string? propertyName = null)
        {
            Contract.Requires<ArgumentNullException>((propertyName is not null), "Argument can't be null!");

            // Remove list with errors for property
            _propertyNameToErrorsDictionary.Remove(propertyName);

            // Raise event to notify that error state has changed
            OnErrorsChanged(propertyName);
        }

        /// <summary>
        /// Raise event to notify about error state change
        /// </summary>
        /// <param name="propertyName">name of property</param>
        private void OnErrorsChanged([CallerMemberName] string? propertyName = null)
        {
            Contract.Requires<ArgumentNullException>((propertyName is not null), "Argument can't be null!");

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raise event to notify about property change
        /// </summary>
        /// <param name="propertyName">name of property</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            Contract.Requires<ArgumentNullException>((propertyName is not null), "Argument can't be null!");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
