using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AB15_GUI.WPF.Models.Interfaces;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class to add observability functionality to register
    /// Note: limited to direct data modification, won't raise event's if only fields of register are modified!
    /// </summary>
    public class ObservableRegister : INotifyPropertyChanged
    {       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="register">ref to register object</param>
        public ObservableRegister(IRegister register)
        {
            this.register = register;
        }

        /// <summary>
        /// <inheritdoc cref="Register" path='/summary'/>
        /// </summary>
        private IRegister register;

        /// <summary>
        /// Observable property for register
        /// </summary>
        /// <value></value>
        public IRegister Register
        {
            get => register;
            set
            {
                register = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Data));
            }
        }

        /// <summary>
        /// Observable data property for register
        /// </summary>
        /// <value></value>
        public ushort Data
        {
            get => register.Data;
            set
            {
                register.Data = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Register));
            }
        }

        #region Services

        /// <summary>
        /// Event for notification if property has changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise event to notify about property change
        /// </summary>
        /// <param name="propertyName">name of property</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Sanity check
            if (propertyName == null) throw new ArgumentException("Property name can't be null!");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // Services
    }
}
