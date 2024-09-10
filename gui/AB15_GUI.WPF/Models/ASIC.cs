using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AB15_GUI.WPF.Services.Interfaces;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// ASIC top level control and status
    /// </summary>
    public class ASIC : INotifyPropertyChanged
    {
        /// <summary>
        /// SerialWrapper reference to perform communication with MCU
        /// </summary>
        private readonly ISerialWrapper serialWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serialWrapper"><inheritdoc cref="serialWrapper" path='/summary'/></param>
        public ASIC(ISerialWrapper serialWrapper)
        {
            // Add reference to serial wrapper
            this.serialWrapper = serialWrapper;
        }



        /// <summary>
        /// <inheritdoc cref="ID" path='/summary'/>
        /// </summary>
        private int id = 0;

        /// <summary>
        /// ASIC ID relative to MCU
        /// 0 - not set, 1 - master, 2-4 - slaves
        /// </summary>
        public int ID 
        { 
            get => id;
            set
            {
                // Check if vaid ASIC ID is received
                if ((value < 0) || (value > 4)) throw new ArgumentOutOfRangeException(nameof(ID), $"Unsupported option for ASIC ID: {value}");

                id = value;
                OnPropertyChanged();
            } 
        }

        /// <summary>
        /// <inheritdoc cref="State" path='/summary'/>
        /// </summary>
        private ASICState state = 0;

        /// <summary>
        /// ASIC state
        /// 0 - not set, 1 - master, 2-4 - slaves
        /// </summary>
        public ASICState State
        { 
            get => state;
            set
            {
                // Check if vaid ASIC state is received
                if (!Enum.IsDefined(typeof(ASICState), value)) throw new ArgumentOutOfRangeException(nameof(ID), $"Unsupported option for ASIC state: {value}");

                state = value;
                OnPropertyChanged();
            } 
        }

        /// <summary>
        /// <inheritdoc cref="EOP" path='/summary'/>
        /// </summary>
        private bool eop = false;

        /// <summary>
        /// ASIC EOP flag. If set most configuration is locked
        /// </summary>
        public bool EOP
        { 
            get => eop;
            set
            {
                eop = value;
                OnPropertyChanged();
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
